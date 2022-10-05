

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
