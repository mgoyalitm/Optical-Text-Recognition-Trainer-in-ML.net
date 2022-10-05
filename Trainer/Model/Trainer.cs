using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.Onnx;
using OpenCvSharp;
using Tensorflow;
using Microsoft.ML.Trainers.LightGbm;
namespace TrainerApp.Model;

public class Trainer : ITrainer
{
    public void LoadModel(string path, EngineType engine)
    {
        Directory.CreateDirectory("models");
        File.Copy(path, $"models\\{engine}.onnx", true);
    }

    public void SaveModel(string path, EngineType engine)
    {
        Directory.CreateDirectory(Path.GetFullPath(path));
        File.Copy($"models\\{engine}.onnx", path);
    }

    public void Delete(EngineType engine) => File.Delete($"models\\{engine}.onnx");

    public void Train(IEnumerable<FontSetting> settings, EngineType engine, ITrainingProgress progress)
    {
        progress.Current = 0;
        progress.Maximum = settings.Sum(x => x.CharCount);
        
        MLContext context = new(seed: null);

        IDataView data = engine switch
        {
            EngineType.ImageClassification => context.Data.LoadFromEnumerable(GetOpticalTrainData(settings, progress)),
            _ => context.Data.LoadFromEnumerable(GetTrainData(settings, progress)),
        };

        EstimatorChain<ColumnConcatenatingTransformer> mapingPipeline = context.Transforms.Conversion.MapValueToKey(inputColumnName: "CharacterId", outputColumnName: "Label")
            .Append(context.Transforms.Concatenate("Features", nameof(CharacterData.Pixels)))
            .AppendCacheCheckpoint(context);
        EstimatorChain<KeyToValueMappingTransformer> pipeline = engine switch
        {
            EngineType.SdcaNonCalibrated => mapingPipeline.Append(context.MulticlassClassification.Trainers.SdcaNonCalibrated("Label", "Features")).Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel")),
            EngineType.SdcaMaximumEntropy => mapingPipeline.Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features")).Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel")),
            EngineType.LbfgsMaximumEntropy => mapingPipeline.Append(context.MulticlassClassification.Trainers.LbfgsMaximumEntropy("Label", "Features")).Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel")),
            EngineType.NaiveBayes => mapingPipeline.Append(context.MulticlassClassification.Trainers.NaiveBayes("Label", "Features")).Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel")),
            EngineType.ImageClassification => mapingPipeline.Append(context.MulticlassClassification.Trainers.ImageClassification("Label", "Features")).Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel")),
            EngineType.LightGmb => mapingPipeline.Append(context.MulticlassClassification.Trainers.LightGbm("Label", "Features")).Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel")),
            _ => throw new NotImplementedException(),
        };
        TransformerChain<KeyToValueMappingTransformer> model = pipeline.Fit(data);
        Directory.CreateDirectory("models");
        context.Model.Save(model, data.Schema, $"models\\{engine}.onnx");
    }

    public void EvaluateAccuracy(FontSetting setting, EngineType engine, IAccuracyProgress progress)
    {
        progress.Current = 0;
        progress.Maximum = setting.CharCount;

        MLContext context = new(seed: null);

        ITransformer model = context.Model.Load($"models\\{engine}.onnx", out DataViewSchema columns);
        Dictionary<int, int> characterDictionary = GetCharDictionary($"models\\{engine}.onnx");
        int correct = 0;
        int total = 0;
        float correctness = 0f;
        if (engine == EngineType.ImageClassification)
        {
            using PredictionEngine<CharacterOpticalData, PredictionData> predictionEngine = context.Model.CreatePredictionEngine<CharacterOpticalData, PredictionData>(model, columns);
            
            foreach (CharacterOpticalData character in GetOpticalTrainData(setting, progress))
            {
                total++;
                PredictionData prediction = predictionEngine.Predict(character);
                UpdateCorrectness(prediction.Score, character.CharacterId);
            }
        }
        else
        {
            using PredictionEngine<CharacterData, PredictionData> predictionEngine = context.Model.CreatePredictionEngine<CharacterData, PredictionData>(model, columns);
    
            foreach (CharacterData character in GetTrainData(setting, progress))
            {
                total++;
                PredictionData prediction = predictionEngine.Predict(character);
                UpdateCorrectness(prediction.Score, character.CharacterId);
               
            }
        }

        progress.Accuracy = float.Round(Convert.ToSingle(correct) / Convert.ToSingle(total), 2);
        progress.AverageScore = float.Round(correctness / Convert.ToSingle(total), 2);

        static int GetCharacter(int id) => (id - (id % Alphabet.CharOffSet)) / Alphabet.CharOffSet;
        void UpdateCorrectness(float[] scores, int actual)
        {
            Dictionary<int, float> scoreDictionary = new();
            for (int i = 0; i < scores.Length; i++)
            {
                int charId = GetCharacter(characterDictionary[i]);
                if (scoreDictionary.ContainsKey(charId) == false)
                {
                    scoreDictionary[charId] = 0;
                }
                scoreDictionary[charId] += scores[i];
            }
            int id = GetCharacter(actual);
            if (scoreDictionary.ContainsKey(id))
            {
                if (scoreDictionary[id] > 0.25)
                {
                    correct++;
                    correctness += scoreDictionary[id];
                }
            }
        }
    }

    public void Evaluate(FontSetting setting, EngineType engine, ITestProgress progress)
    {
        progress.Current = 0;
        progress.Maximum = setting.CharCount;

        MLContext context = new(seed: null);

        IDataView data = engine switch
        {
            EngineType.ImageClassification => context.Data.LoadFromEnumerable(GetOpticalTrainData(setting, progress)),
            _ => context.Data.LoadFromEnumerable(GetTrainData(setting, progress)),
        };
        ITransformer model = context.Model.Load($"models\\{engine}.onnx", out DataViewSchema columns);

        if (engine == EngineType.ImageClassification)
        {
            using PredictionEngine<CharacterOpticalData, PredictionData> predictionEngine = context.Model.CreatePredictionEngine<CharacterOpticalData, PredictionData>(model, columns);
            MulticlassClassificationMetrics metric = context.MulticlassClassification.Evaluate(model.Transform(data));
            progress.IsEvaluated = true;
            progress.LogLoss = double.Round(metric.LogLoss, 2);
            progress.LogLossReduction = double.Round(metric.LogLossReduction, 2);
            progress.MicroAccuracy = double.Round(metric.MicroAccuracy, 2);
            progress.MacroAccuracy = double.Round(metric.MacroAccuracy, 2);
        }
        else
        {
            using PredictionEngine<CharacterData, PredictionData> predictionEngine = context.Model.CreatePredictionEngine<CharacterData, PredictionData>(model, columns);
            MulticlassClassificationMetrics metric = context.MulticlassClassification.Evaluate(model.Transform(data));
            progress.IsEvaluated = true;
            progress.LogLoss = double.Round(metric.LogLoss, 2);
            progress.LogLossReduction = double.Round(metric.LogLossReduction, 2);
            progress.MicroAccuracy = double.Round(metric.MicroAccuracy, 2);
            progress.MacroAccuracy = double.Round(metric.MacroAccuracy, 2);
        }
    }
    private static Dictionary<int, int> GetCharDictionary(string path)
    {
        Dictionary<int, int> dict = new();
        using ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Read);

        foreach (ZipArchiveEntry entry in zip.Entries)
        {
            if (entry.Name == "Terms.txt")
            {
                using Stream stream = entry.Open();
                using StreamReader reader = new(stream);

                foreach (int[] pair in GetLines(reader).Skip(1).Select(x => x.Split('\t').Select(n => int.Parse(n.Trim())).Take(2).ToArray()))
                {
                    dict[pair[0]] = pair[1];
                }

                static IEnumerable<string> GetLines(StreamReader reader)
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }

                }
            }
        }

        return dict;
    }
    private static IEnumerable<CharacterData> GetTrainData(FontSetting setting, IProgress progress)
    {
        foreach (Alphabet alphabet in setting.GetAlphabets())
        {
            using Mat mat = alphabet.Print();
            mat.GetArray(out byte[] data);
            progress.Current++;
            yield return new CharacterData
            {
                CharacterId = alphabet.CharacterId,
                Pixels = data.Select(x => x == 0 ? 0f : 1f).ToArray()
            };
        }
    }

    private static IEnumerable<CharacterData> GetTrainData(IEnumerable<FontSetting> settings, IProgress progress)
    {
        foreach (FontSetting setting in settings)
        {
            foreach (CharacterData data in GetTrainData(setting, progress))
            {
                yield return data;
            }
        }
    }

    private static IEnumerable<CharacterOpticalData> GetOpticalTrainData(FontSetting setting, IProgress progress)
    {
        foreach (Alphabet alphabet in setting.GetAlphabets())
        {
            using Mat mat = alphabet.Print();

            progress.Current++;
            yield return new CharacterOpticalData
            {
                CharacterId = alphabet.CharacterId,
                Pixels = mat.ToBytes(".jpg")
            };
        }
    }

    private static IEnumerable<CharacterOpticalData> GetOpticalTrainData(IEnumerable<FontSetting> settings, IProgress progress)
    {
        foreach (FontSetting setting in settings)
        {
            foreach (CharacterOpticalData data in GetOpticalTrainData(setting, progress))
            {
                yield return data;
            }
        }
    }

}
