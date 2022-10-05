namespace TrainerApp.ViewModel.Commands;

public class RemoveFontSettingsCommand : ICommand
{
    public RemoveFontSettingsCommand(TrainerViewModel trainer) => _trainer = trainer;
    private readonly TrainerViewModel _trainer;
    public event EventHandler CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object _) => true;

    public void Execute(object _)
    {
        for (int index = 0; index < _trainer.TrainingFontSettingList.Count; index++)
        {
            if (_trainer.TrainingFontSettingList[index].FontName == _trainer.FontSetting.FontName)
            {
                _trainer.TrainingFontSettingList.RemoveAt(index);
                return;
            }
        }
    }
}
