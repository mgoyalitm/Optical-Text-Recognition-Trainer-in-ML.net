namespace TrainerApp.ViewModel.Commands;

public class ClearFontSettingsCommand : ICommand
{
    private readonly TrainerViewModel _viewmodel;
    public ClearFontSettingsCommand(TrainerViewModel viewmodel) => _viewmodel = viewmodel;

    public event EventHandler CanExecuteChanged { add { } remove { } }

    public bool CanExecute(object _) => true;

    public void Execute(object _) => _viewmodel?.TrainingFontSettingList?.Clear();
}
