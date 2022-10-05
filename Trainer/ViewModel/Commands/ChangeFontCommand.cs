using OpenCvSharp;
using System.Drawing;

namespace TrainerApp.ViewModel.Commands;

public class ChangeFontCommand : ICommand
{
    public ChangeFontCommand(TrainerViewModel trainer)
    {
        _trainer = trainer;
    }

    private readonly TrainerViewModel _trainer;
    
    public event EventHandler CanExecuteChanged;
    public bool CanExecute(object parameter) => true;
    public void Execute(object parameter)
    {
        if (parameter is int step)
        {
            _trainer.SelectedFontIndex += step;
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
