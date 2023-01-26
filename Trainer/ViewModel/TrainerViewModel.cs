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

namespace TrainerApp.ViewModel;
public class TrainerViewModel : INotifyPropertyChanged
{
    public TrainerViewModel()
    {
        AvailableFonts = GetFontNames().OrderBy(x => x).ToList();
        FontSetting = new();
        SelectedFontIndex = 0;
        ChangeFontCommand = new(this);
        TrainingFontSettingList = new();
        AddFontSettingCommand = new (this);
        RemoveFontSettingCommand = new (this);
        EngineSetting = new ();
        TrainModelCommand = new(this);
        EvaluateModelCommand = new(this);
        CheckAccuracyCommand = new (this);
        ClearFontSettingsCommand = new (this);
        SaveSettingsCommand = new (this);

        AppSettings.LoadSettings(this);
    }

    private int selectedFontIndex = -1;
    public List<string> AvailableFonts { get; }
    public EngineSetting EngineSetting { get; }
    public FontSettingViewModel FontSetting { get; }
    public AddFontSettingCommand AddFontSettingCommand { get; }
    public RemoveFontSettingsCommand RemoveFontSettingCommand { get; }
    public ObservableCollection<FontSetting> TrainingFontSettingList { get; }
    public ChangeFontCommand ChangeFontCommand { get; }
    public TrainModelCommand TrainModelCommand { get; }
    public EvaluateModelCommand EvaluateModelCommand { get; }
    public CheckAccuracyCommand CheckAccuracyCommand { get; }
    public ClearFontSettingsCommand ClearFontSettingsCommand { get; }
    public SaveSettingsCommand SaveSettingsCommand { get; }
    public string SelectedFont => AvailableFonts[SelectedFontIndex];
    public int SelectedFontIndex
    {
        get => selectedFontIndex;
        set
        {
            if (value == selectedFontIndex) return;

            selectedFontIndex = value switch
            {
                int n when (n >= AvailableFonts.Count) => 0,
                int n when (n < 0) => AvailableFonts.Count - 1,
                _ => value,
            };

            FontSetting.FontName = SelectedFont;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedFont));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public static IEnumerable<string> GetFontNames(string culture = "en-us")
    {
        foreach (FontFamily fontfamily in Fonts.SystemFontFamilies)
        {
            foreach (string font in fontfamily.FamilyNames
                .Where(x => x.Key.ToString().Trim().ToLower() == culture)
                .Select(x => x.Value).Distinct())
            {
                yield return font;
            }
        }
    }
}
