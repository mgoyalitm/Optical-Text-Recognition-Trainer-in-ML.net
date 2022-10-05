global using Microsoft.ML;
global using Microsoft.ML.Data;
global using Microsoft.ML.Trainers;
global using Microsoft.ML.Transforms;
global using NumSharp.Extensions;
global using OpenCvSharp;
global using OpenCvSharp.ML;
global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.IO;
global using System.IO.Compression;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using System.Windows.Controls;
global using System.Windows.Forms;
global using Tensorflow.Keras.Engine;
using static OpenCvSharp.LineIterator;

namespace CharacterRecognition.Trainer
{
    public class OpticalCharacterData
    {
        [LoadColumn(0)]
        public float Character { get; set; }
        public byte[] PixelValues { get; set; }
    }

    public class CharacterData
    {
        [LoadColumn(0)]
        public float Character { get; set; }
        [LoadColumn(1, 1024), VectorType(1024)]
        public float[] PixelValues { get; set; }
    }

    public class CharacterPrediction
    {
        [ColumnName("PredictedLabel")]
        public float Character { get; set; }
        [ColumnName("Score")]
        public float[] Score { get; set; }
    }

    public static class CharacterTrainer
    {
        private static Dictionary<int, char> GetCharDictionary(string model)
        {
            Dictionary<int, char> dict = new();
            using ZipArchive zip = ZipFile.Open($"{model}.onnx", ZipArchiveMode.Read);
            
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                if (entry.Name == "Terms.txt")
                {
                    using Stream stream = entry.Open();
                    using StreamReader reader = new(stream);



                    foreach (int[] pair in GetLines(reader).Skip(1).Select(x => x.Split('\t').Select(n => int.Parse(n.Trim())).Take(2).ToArray()))
                    {
                        dict[pair[0]] = (char)(pair[1] + '0');
                    } 

                    static IEnumerable<string> GetLines(StreamReader reader)
                    {
                        string line;
                        while (( line = reader.ReadLine()) != null)
                        {
                            yield return line;
                        }

                    }
                }
            }

            return dict;
        }
       

        public static void TrainImageClassificationData(string folder, string modelName, int[] rows, bool test = true)
        {
            MLContext context = new();
            var traindata = context.Data.LoadFromEnumerable(ImageDataBuilder.GetImageData(folder, rows));
            var partition = context.Data.TrainTestSplit(traindata, 0.2);
            EstimatorChain<KeyToValueMappingTransformer> pipeline = context.Transforms.Conversion.MapValueToKey(inputColumnName: "Character", outputColumnName: "Label")
                    .Append(context.Transforms.Concatenate("Features", "PixelValues"))
                    .AppendCacheCheckpoint(context)
                    .Append(context.MulticlassClassification.Trainers.LbfgsMaximumEntropy("Label", "Features"))
                    .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Training Model");
            TransformerChain<KeyToValueMappingTransformer> model = pipeline.Fit(test ? partition.TrainSet : traindata);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Training Completed");

            if (test == true)
            {
                MulticlassClassificationMetrics metric = context.MulticlassClassification.Evaluate(model.Transform(partition.TestSet));
                Console.ForegroundColor = ConsoleColor.Green;


                Console.WriteLine($"Micro Accuracy:         {metric.MicroAccuracy:0.###}");
                Console.WriteLine($"Macro Accuracy:         {metric.MacroAccuracy:0.###}");
                Console.WriteLine();

                Console.WriteLine($"Log Loss:               {metric.LogLoss:#.###}");
                Console.WriteLine($"Log Loss Reduction:     {metric.LogLossReduction:#.###}");
                Console.WriteLine();

                Console.WriteLine($"Peak Accuracy:          {metric.TopKAccuracy:###.###}");
                Console.WriteLine($"Peak Count:             {metric.TopKPredictionCount}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                context.Model.Save(model, traindata.Schema, $"{modelName}.onnx");
            }

        }
        public static void InterractiveTesting(string csvPath, string modelName)
        {
            MLContext context = new();
            ITransformer model = context.Model.Load($"{modelName}.onnx", out DataViewSchema schema);
            
            

            PredictionEngine<CharacterData, CharacterPrediction> engine = context.Model.CreatePredictionEngine<CharacterData, CharacterPrediction>(model);

            Dictionary<int, char> dictionary = GetCharDictionary(modelName);
            Dictionary<char, int> reverse = dictionary.ToDictionary(x => x.Value, y => y.Key);

            
            Console.WriteLine("Intializing Data...");
            
            IDataView data = context.Data.LoadFromTextFile<CharacterData>(csvPath, hasHeader:false, separatorChar: ',');
            int skips = 806 * 8;
            float[] chars = data.GetColumn<float>("Character").Skip(skips).Take(806).ToArray();
            List<float[]> pixels = data.GetColumn<float[]>("PixelValues").Skip(skips).Take(806).ToList();

            for (int i = 0; i < chars.Length; i++)
            {
                CharacterData testdata = new()
                {
                    Character = chars[i],
                    PixelValues = pixels[i]
                };
                
                CharacterPrediction predictionData = engine.Predict(testdata);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
                float confidence = predictionData.Score[reverse[(char)(predictionData.Character  + '0')]];
                Console.WriteLine($"Actual | Predicted | Confidence : {FloatToChar(testdata.Character)} {FloatToChar(predictionData.Character)} : {confidence}");

                Console.ForegroundColor = predictionData.Character == testdata.Character ? ConsoleColor.DarkGreen : ConsoleColor.Red;

                for (int scoreIndex = 0; scoreIndex < predictionData.Score.Length; scoreIndex++)
                {
                    string score = $"{predictionData.Score[scoreIndex]:0.#####}";
                    if (string.IsNullOrWhiteSpace(score) == false && predictionData.Score[scoreIndex] > 0.1)
                    {
                        Console.WriteLine($"Character : {dictionary[scoreIndex]} : {score}");
                    }
                }
                Thread.Sleep(100);
                using Mat pixel = new(1, 1, MatType.CV_8UC1, new Scalar(0));
                using Mat mat = new(33, 33, MatType.CV_8UC1, new Scalar(255));
                for (int index = 0; index < testdata.PixelValues.Length; index++)
                {
                    if (testdata.PixelValues[index] == 1)
                    {
                        int xCord = index % 32;
                        int yCord = index / 32;
                        pixel.CopyTo(mat[yCord, yCord + 1, xCord, xCord + 1]);
                    }

                }
                Cv2.ImShow($"Char = {testdata.Character}", mat);
                Cv2.WaitKey();
            }
        }

        public static char FloatToChar(float value) => (char)((int)(value + '0'));
        public static void TrainAndSave(string csvPath, string outputModelName, bool test = true)
        {

            MLContext context = new();
            
            IDataView traindata = context.Data.LoadFromTextFile<CharacterData>(path: csvPath, hasHeader: false, separatorChar: ',');
            EstimatorChain<KeyToValueMappingTransformer> pipeline = context.Transforms.Conversion.MapValueToKey(inputColumnName: "Character", outputColumnName: "Label")
                .Append(context.Transforms.Concatenate("Features", "PixelValues"))
                .AppendCacheCheckpoint(context)
                .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            var partition = context.Data.TrainTestSplit(traindata, test ? 0.2d : double.Epsilon);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Training Model");
            TransformerChain<KeyToValueMappingTransformer> model = pipeline.Fit(partition.TrainSet);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Training Completed");


            if (test == true)
            {
                MulticlassClassificationMetrics metric = context.MulticlassClassification.Evaluate(model.Transform(partition.TestSet));
                Console.ForegroundColor = ConsoleColor.Green;


                Console.WriteLine($"Micro Accuracy:         {metric.MicroAccuracy:0.###}");
                Console.WriteLine($"Macro Accuracy:         {metric.MacroAccuracy:0.###}");
                Console.WriteLine();

                Console.WriteLine($"Log Loss:               {metric.LogLoss:#.###}");
                Console.WriteLine($"Log Loss Reduction:     {metric.LogLossReduction:#.###}");
                Console.WriteLine();

                Console.WriteLine($"Peak Accuracy:          {metric.TopKAccuracy:###.###}");
                Console.WriteLine($"Peak Count:             {metric.TopKPredictionCount}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                context.Model.Save(model, traindata.Schema, $"{outputModelName}.onnx");
            }
        }

        public static void ImageClassificationInterractiveTesting(string folder, string modelName,  int[] rows)
        {
            MLContext context = new(seed: 0);
            ITransformer model = context.Model.Load($"{modelName}.onnx", out DataViewSchema columns);
            PredictionEngine<OpticalCharacterData, CharacterPrediction> engine = context.Model.CreatePredictionEngine<OpticalCharacterData, CharacterPrediction>(model);
            
            //var traindata = context.Data.LoadFromEnumerable(ImageDataBuilder.GetImageData(folder, rows).Skip(168).Take(56));

            //MulticlassClassificationMetrics metric = context.MulticlassClassification.Evaluate(model.Transform(traindata));
            //Console.ForegroundColor = ConsoleColor.Green;


            //Console.WriteLine($"Micro Accuracy:         {metric.MicroAccuracy:0.###}");
            //Console.WriteLine($"Macro Accuracy:         {metric.MacroAccuracy:0.###}");
            //Console.WriteLine();

            //Console.WriteLine($"Log Loss:               {metric.LogLoss:#.###}");
            //Console.WriteLine($"Log Loss Reduction:     {metric.LogLossReduction:#.###}");
            //Console.WriteLine();

            //Console.WriteLine($"Peak Accuracy:          {metric.TopKAccuracy:###.###}");
            //Console.WriteLine($"Peak Count:             {metric.TopKPredictionCount}");
            //Console.ForegroundColor = ConsoleColor.White;

            Dictionary<int, char> dictionary = GetCharDictionary(modelName);
            Dictionary<char, int> reverse = dictionary.ToDictionary(x => x.Value, y => y.Key);

            foreach (OpticalCharacterData data in ImageDataBuilder.GetImageData(folder, rows))
            {
                CharacterPrediction predictionData = engine.Predict(data);
                float confidence = predictionData.Score[reverse[(char)(predictionData.Character + '0')]];

                Console.ForegroundColor = ConsoleColor.White;
                //Console.ForegroundColor = predictionData.Character == data.Character ? ConsoleColor.DarkGreen : ConsoleColor.Red;
                Console.WriteLine($"Actual | Predicted | Confidence : {FloatToChar(data.Character)} | {FloatToChar(predictionData.Character)} | {confidence}");
                //using Mat mat = Mat.FromImageData(data.PixelValues);
                //Cv2.ImShow($"{data.Character}", mat);
                //Cv2.WaitKey();
                if (predictionData.Character != data.Character)
                {
                    Console.ReadKey();
                }
            }

        }
    }
}
