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

public class FontSettingViewModel : INotifyPropertyChanged, IFontSetting
{
    private bool useNormalFont = true;
    private bool useBoldFont = true;
    private bool useItalicFont = true;
    private bool useBoldItalicFont = true;
    private string fontName = "Arial";
    private bool useLowerCaseLetters = true;
    private bool useUpperCaseLetters = true;
    private bool useNumbers = true;
    private bool useNormalFontRotation = true;
    private double normalFontMinRotation = -10d;
    private double normalFontMaxRotation = 10d;
    private bool useItalicFontRotation = true;
    private double italicFontMinRotation = -15;
    private double italicFontMaxRotation = 5;

    public string FontName
    {
        get => fontName;
        set
        {
            if (value != fontName)
            {
                fontName = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseNormalFont
    {
        get => useNormalFont;
        set
        {
            if (value != useNormalFont)
            {
                useNormalFont = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseBoldFont
    {
        get => useBoldFont;
        set
        {
            if (value != useBoldFont)
            {
                useBoldFont = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseItalicFont
    {
        get => useItalicFont;
        set
        {
            if (value != useItalicFont)
            {
                useItalicFont = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseBoldItalicFont
    {
        get => useBoldItalicFont;
        set
        {
            if (value != useBoldItalicFont)
            {
                useBoldItalicFont = value;
                OnPropertyChanged();
            }
        }
    }


    public bool UseLowerCaseLetters
    {
        get => useLowerCaseLetters;
        set
        {
            if (value != useLowerCaseLetters)
            {
                useLowerCaseLetters = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseUpperCaseLetters
    {
        get => useUpperCaseLetters;
        set
        {
            if (value != useUpperCaseLetters)
            {
                useUpperCaseLetters = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseNumbers
    {
        get => useNumbers;
        set
        {
            if (value != useNumbers)
            {
                useNumbers = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseNormalFontRotation
    {
        get => useNormalFontRotation;
        set
        {
            if (value != useNormalFontRotation)
            {
                useNormalFontRotation = value;
                OnPropertyChanged();
            }
        }
    }

    public double NormalFontMinRotation
    {
        get => normalFontMinRotation;
        set
        {
            if (value != normalFontMinRotation)
            {
                normalFontMinRotation = value;
                OnPropertyChanged();
            }
        }
    }

    public double NormalFontMaxRotation
    {
        get => normalFontMaxRotation;
        set
        {
            if (value != normalFontMaxRotation)
            {
                normalFontMaxRotation = value;
                OnPropertyChanged();
            }
        }
    }

    public bool UseItalicFontRotation
    {
        get => useItalicFontRotation;
        set
        {
            if (value != useItalicFontRotation)
            {
                useItalicFontRotation = value;
                OnPropertyChanged();
            }
        }
    }

    public double ItalicFontMinRotation
    {
        get => italicFontMinRotation;
        set
        {
            if (value != italicFontMinRotation)
            {
                italicFontMinRotation = value;
                OnPropertyChanged();
            }
        }
    }

    public double ItalicFontMaxRotation 
    { 
        get => italicFontMaxRotation; 
        set 
        {
            if (value != italicFontMaxRotation)
            {
                italicFontMaxRotation = value;
                OnPropertyChanged();
            }
        } 
    }


    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
