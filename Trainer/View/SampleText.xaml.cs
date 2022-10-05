using System.Windows;

namespace TrainerApp.View;
public partial class SampleText : UserControl
{
    public SampleText() => InitializeComponent();

    public static readonly DependencyProperty ShowLowerCaseProperty =  DependencyProperty.Register(nameof(ShowLowerCase), typeof(bool), typeof(SampleText), new PropertyMetadata(true));
    public static readonly DependencyProperty ShowUpperCaseProperty = DependencyProperty.Register(nameof(ShowUpperCase), typeof(bool), typeof(SampleText), new PropertyMetadata(true));
    public static readonly DependencyProperty ShowNumericsProperty = DependencyProperty.Register(nameof(ShowNumerics), typeof(bool), typeof(SampleText), new PropertyMetadata(true));
    public bool ShowLowerCase
    {
        get => (bool)GetValue(ShowLowerCaseProperty);
        set => SetValue(ShowLowerCaseProperty, value);
    }

    public bool ShowUpperCase
    {
        get => (bool)GetValue(ShowUpperCaseProperty);
        set => SetValue(ShowUpperCaseProperty, value);
    }

    public bool ShowNumerics
    {
        get => (bool)GetValue(ShowNumericsProperty);
        set => SetValue(ShowNumericsProperty, value);
    }
}