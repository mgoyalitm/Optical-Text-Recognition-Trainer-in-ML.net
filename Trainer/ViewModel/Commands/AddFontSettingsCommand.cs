namespace TrainerApp.ViewModel.Commands;

public class AddFontSettingCommand : ICommand
{
    public AddFontSettingCommand(TrainerViewModel trainer) => _trainer = trainer;
    private readonly TrainerViewModel _trainer;
    public event EventHandler CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object _) => true;
    public void Execute(object _)
    {
        for (int index = 0; index < _trainer.TrainingFontSettingList.Count; index++)
        {
            if (_trainer.FontSetting.FontName == _trainer.TrainingFontSettingList[index].FontName)
            {
                return;
            }
        }

        _trainer.TrainingFontSettingList.Add(new FontSetting(_trainer.FontSetting));
    }
}
