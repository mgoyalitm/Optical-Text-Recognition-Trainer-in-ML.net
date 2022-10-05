namespace TrainerApp.ViewModel;

public class EngineSetting : INotifyPropertyChanged
{
    private bool useSdcaMaximumEntropy = true;
    private bool useSdcaNonCalibrated;
    private bool useLbfgsMaximumEntropy;
    private bool useNaiveBayes;
    private bool useImageClassification;
    private bool useLightGmb;
    private EngineType selectedEngineType = EngineType.SdcaMaximumEntropy;

    public EngineType SelectedEngineType
    {
        get => selectedEngineType;
        set
        {
            if (value != selectedEngineType)
            {
                selectedEngineType = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseSdcaMaximumEntropy
    {
        get => useSdcaMaximumEntropy;
        set
        {
            if (value != useSdcaMaximumEntropy)
            {
                useSdcaMaximumEntropy = value;
                OnPropertyChanged();

                if (value == true)
                {
                    SelectedEngineType = EngineType.SdcaMaximumEntropy;
                }
            }
        }
    }
    
    public bool UseSdcaNonCalibrated
    {
        get => useSdcaNonCalibrated;
        set
        {
            if (value != useSdcaNonCalibrated)
            {
                useSdcaNonCalibrated = value;
                OnPropertyChanged();

                if (value == true)
                {
                    SelectedEngineType = EngineType.SdcaNonCalibrated;
                }
            }
        }
    }

    public bool UseLbfgsMaximumEntropy
    {
        get => useLbfgsMaximumEntropy;
        set
        {
            if (value != useLbfgsMaximumEntropy)
            {
                useLbfgsMaximumEntropy = value;
                OnPropertyChanged();

                if (value == true)
                {
                    SelectedEngineType = EngineType.LbfgsMaximumEntropy;
                }
            }

        }
    }
    
    public bool UseNaiveBayes
    {
        get => useNaiveBayes;
        set
        {
            if (value != useNaiveBayes)
            {
                useNaiveBayes = value;
                OnPropertyChanged();

                if (value == true)
                {
                    SelectedEngineType = EngineType.NaiveBayes;
                }
            }
        }
    }

    public bool UseImageClassification
    {
        get => useImageClassification;
        set
        {
            if (value != useImageClassification)
            {
                useImageClassification = value;
                OnPropertyChanged();

                if (value == true)
                {
                    SelectedEngineType = EngineType.ImageClassification;
                }
            }
        }
    }

    public bool UseLightGmb
    {
        get => useLightGmb;
        set
        {
            if (value != useLightGmb)
            {
                useLightGmb = value;
                OnPropertyChanged();

                if (value == true)
                {
                    SelectedEngineType = EngineType.LightGmb;
                }
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
