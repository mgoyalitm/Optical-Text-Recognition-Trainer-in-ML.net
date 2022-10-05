using System.Windows;

namespace TrainerApp.View;
public partial class TrainerWindow : Window
{
    public TrainerWindow() => InitializeComponent();
    protected override void OnClosing(CancelEventArgs e)
    {
        if (DataContext is TrainerViewModel trainer)
        {
            AppSettings.SaveSettings(trainer);
        }
        base.OnClosing(e);
    }
}
