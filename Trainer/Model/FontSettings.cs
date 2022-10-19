namespace TrainerApp.Model;

public readonly record struct FontSetting(
    string FontName,
    bool UseNormalFont,
    bool UseBoldFont,
    bool UseItalicFont,
    bool UseBoldItalicFont,
    bool UseLowerCaseLetters,
    bool UseUpperCaseLetters,
    bool UseNumbers,
    bool UseNormalFontRotation,
    double NormalFontMinRotation,
    double NormalFontMaxRotation,
    bool UseItalicFontRotation,
    double ItalicFontMinRotation,
    double ItalicFontMaxRotation) : IFontSetting
{
    public FontSetting(IFontSetting settings) : this(settings.FontName, settings.UseNormalFont, settings.UseBoldFont, settings.UseItalicFont, settings.UseBoldItalicFont, settings.UseLowerCaseLetters, settings.UseUpperCaseLetters, settings.UseNumbers, settings.UseNormalFontRotation, settings.NormalFontMinRotation, settings.NormalFontMaxRotation, settings.UseItalicFontRotation, settings.ItalicFontMinRotation, settings.ItalicFontMaxRotation) { }

    public const double RotationStep = 4.2d;
    public const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
    public const string Numbers = "0123456789\\/-_⟋⟍⧵⁄╱╲⬥.<>‹› ";
    public int CharCount
    {
        get
        {
            int stylesCount = 0;
            if (UseNormalFont) stylesCount++;
            if (UseBoldFont) stylesCount++;
            if (UseItalicFont) stylesCount++;
            if (UseBoldItalicFont) stylesCount++;

            int charCount = 0;

            if (UseLowerCaseLetters) charCount += 26;
            if (UseUpperCaseLetters) charCount += 26;
            if (UseNumbers) charCount += 27;

            int rotationsCount = 0;

            if (UseNormalFontRotation)
            {
                rotationsCount += (int)((NormalFontMaxRotation - NormalFontMinRotation) / RotationStep) + 1;
            }
            else
            {
                rotationsCount++;
            }

            if (UseItalicFontRotation)
            {
                rotationsCount += (int)((ItalicFontMaxRotation - ItalicFontMinRotation) / RotationStep) + 1;
            }
            else
            {
                rotationsCount++;
            }

            return stylesCount * charCount * rotationsCount;
        }
    }

    public IEnumerable<Alphabet> GetAlphabets()
    {
        foreach (char character in GetLetters())
        {
            foreach (double rotation in GetNormalFontRotations())
            {
                if (UseNormalFont == true)
                {
                    yield return new Alphabet(FontName, character, DrawStyle.Normal, rotation);
                }

                if (UseBoldFont == true)
                {
                    yield return new Alphabet(FontName, character, DrawStyle.Bold, rotation);
                }
            }

            foreach (double rotation in GetItalicFontRotations())
            {
                if (UseItalicFont == true)
                {
                    yield return new Alphabet(FontName, character, DrawStyle.Italic, rotation);
                }

                if (UseBoldItalicFont == true)
                {
                   yield return new Alphabet(FontName, character, DrawStyle.BoldItalic, rotation);
                }
            }
        }
    }

    private IEnumerable<double> GetNormalFontRotations()
    {
        if (UseNormalFontRotation)
        {
            double offset = ((NormalFontMaxRotation - NormalFontMinRotation) % RotationStep) / 2d;
            for (double rotation = NormalFontMinRotation; rotation <= NormalFontMaxRotation; rotation += RotationStep)
            {
                yield return rotation + offset;
            }
        }
        else
        {
            yield return 0;
        }
    }

    private IEnumerable<double> GetItalicFontRotations()
    {
        if (UseItalicFontRotation)
        {
            double offset = ((ItalicFontMaxRotation - ItalicFontMinRotation) % RotationStep) / 2d;
            for (double rotation = ItalicFontMinRotation; rotation <= ItalicFontMaxRotation; rotation += RotationStep)
            {
                yield return rotation + offset;
            }
        }
        else
        {
            yield return 0;
        }
    }

    private IEnumerable<char> GetLetters()
    {
        if (UseLowerCaseLetters)
        {
            foreach (char letter in UpperCaseLetters)
            {
                yield return letter;
            }
        }

        if (UseUpperCaseLetters)
        {
            foreach (char letter in LowerCaseLetters)
            {
                yield return letter;
            }
        }

        if (UseNumbers)
        {
            foreach (char number in Numbers)
            {
                yield return number;
            }
        }
    }
}