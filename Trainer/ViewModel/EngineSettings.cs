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
                    SelectedEngineType = EngineType.LightGbm;
                }
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
