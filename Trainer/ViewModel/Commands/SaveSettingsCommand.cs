namespace TrainerApp.ViewModel.Commands;

public class SaveSettingsCommand : ICommand
{
    public SaveSettingsCommand(TrainerViewModel viewmodel) => _viewmodel = viewmodel;
    private readonly TrainerViewModel _viewmodel;
    public event EventHandler CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object _) => true;   
    public void Execute(object _) => AppSettings.SaveSettings(_viewmodel);
}