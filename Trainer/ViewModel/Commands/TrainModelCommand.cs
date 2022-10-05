using System.Windows;
using TrainerApp.Model;

namespace TrainerApp.ViewModel.Commands;

public class TrainModelCommand : ITrainingProgress, ICommand, INotifyPropertyChanged
{
    public TrainModelCommand(TrainerViewModel viewmodel) => _viewmodel = viewmodel;
    private int maximum = 100;
    private int current;
    private bool isRunning;
    private EngineType engine;

    public int Maximum
    {
        get => maximum;
        set
        {
            if (maximum != value)
            {
                maximum = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(IsTraining));
            }
        }
    }

    public int Current
    {
        get => current;
        set
        {
            if (current != value)
            {
                current = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(IsTraining));
            }
        }
    }

    public bool IsRunning
    {
        get => isRunning;
        set
        {
            if (isRunning != value)
            {
                isRunning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(IsTraining));
            }
        }
    }
    public EngineType Engine
    {
        get => engine;
        set
        {
            if (engine != value)
            {
                engine = value;
                OnPropertyChanged();
            }
        }
    }
    public bool IsLoading => IsRunning && current < maximum;
    public bool IsTraining => IsRunning && current == maximum;

    private readonly TrainerViewModel _viewmodel;

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



    public event EventHandler CanExecuteChanged;
    public bool CanExecute(object parameter) => IsRunning == false;

    public async void Execute(object parameter)
    {

        ITrainer trainer = new Trainer();
        IEnumerable<FontSetting> settings = _viewmodel.TrainingFontSettingList.ToArray();
        Engine = _viewmodel.EngineSetting.SelectedEngineType;
        try
        {
            IsRunning = true;
            if (settings.Sum(x => x.CharCount) < 2)
            {
                MessageBox.Show($"Empty character data");
                return;
            }
            CanExecuteChanged?.Invoke(this, new EventArgs());
            await Task.Run(() => trainer.Train(settings, engine, this));
        }
        catch (NotImplementedException)
        {
            MessageBox.Show($"Engine mode {engine} is not supported.");
        }
        finally
        {
            Current = 0;
            IsRunning = false;
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
