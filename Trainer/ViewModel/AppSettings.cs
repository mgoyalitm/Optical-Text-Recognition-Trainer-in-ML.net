/*
 MIT License

Copyright (c) 2023 Mahendra Goyal

Permission is hereby granted, free of charge, to any person obtaining 
a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
OTHER DEALINGS IN THE SOFTWARE.
 */

using Newtonsoft.Json;

namespace TrainerApp.ViewModel;

public class AppSettings
{
    public const string SettingsPath = "settings.json";
    public string SelectedFont { get; set; }
    public FontSetting[] SelectedFonts { get; set; }
    public FontSetting FontSetting { get; set; }
    public EngineType SelectedEngine { get; set; }

    public static AppSettings ReadFromConfig() => JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(SettingsPath));

    public static void LoadSettings(TrainerViewModel viewmodel)
    {
        if (File.Exists(SettingsPath))
        {
            AppSettings settings = ReadFromConfig();

            switch (settings.SelectedEngine)
            {
                case EngineType.SdcaNonCalibrated:
                    viewmodel.EngineSetting.UseSdcaNonCalibrated = true;
                    break;
                case EngineType.LbfgsMaximumEntropy:
                    viewmodel.EngineSetting.UseLbfgsMaximumEntropy = true;
                    break;
                case EngineType.NaiveBayes:
                    viewmodel.EngineSetting.UseNaiveBayes = true;
                    break;
                case EngineType.ImageClassification:
                    viewmodel.EngineSetting.UseImageClassification = true;
                    break;
                case EngineType.LightGbm:
                    viewmodel.EngineSetting.UseLightGmb = true;
                    break;
                case EngineType.SdcaMaximumEntropy:
                default:
                    viewmodel.EngineSetting.UseSdcaMaximumEntropy = true;
                    break;
            }

            viewmodel.TrainingFontSettingList.Clear();

            foreach (FontSetting setting in settings.SelectedFonts)
            {
                viewmodel.TrainingFontSettingList.Add(setting);
            }

            viewmodel.FontSetting.FontName = settings.FontSetting.FontName;
            viewmodel.FontSetting.UseNormalFont = settings.FontSetting.UseNormalFont;
            viewmodel.FontSetting.UseBoldFont = settings.FontSetting.UseBoldFont;
            viewmodel.FontSetting.UseItalicFont = settings.FontSetting.UseItalicFont;
            viewmodel.FontSetting.UseBoldItalicFont = settings.FontSetting.UseBoldItalicFont;
            viewmodel.FontSetting.UseUpperCaseLetters = settings.FontSetting.UseUpperCaseLetters;
            viewmodel.FontSetting.UseLowerCaseLetters = settings.FontSetting.UseLowerCaseLetters;
            viewmodel.FontSetting.UseNumbers = settings.FontSetting.UseNumbers;
            viewmodel.FontSetting.UseNormalFontRotation = settings.FontSetting.UseNormalFontRotation;
            viewmodel.FontSetting.NormalFontMinRotation = settings.FontSetting.NormalFontMinRotation;
            viewmodel.FontSetting.NormalFontMaxRotation = settings.FontSetting.NormalFontMaxRotation;
            viewmodel.FontSetting.UseItalicFontRotation = settings.FontSetting.UseItalicFontRotation;
            viewmodel.FontSetting.ItalicFontMinRotation = settings.FontSetting.ItalicFontMinRotation;
            viewmodel.FontSetting.ItalicFontMaxRotation = settings.FontSetting.ItalicFontMaxRotation;

            viewmodel.SelectedFontIndex = viewmodel.AvailableFonts.IndexOf(settings.SelectedFont);
        }
    }
    public static void SaveSettings(TrainerViewModel viewmodel)
    {
        AppSettings settings = new()
        {
            SelectedEngine = viewmodel.EngineSetting.SelectedEngineType,
            FontSetting = new(viewmodel.FontSetting),
            SelectedFonts = viewmodel.TrainingFontSettingList.ToArray(),
            SelectedFont = viewmodel.SelectedFont
        };

        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(SettingsPath, json);
    }
}
