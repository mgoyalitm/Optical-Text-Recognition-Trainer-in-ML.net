using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;

namespace TrainerApp.ViewModel.Commands;
public class CheckAccuracyCommand : IAccuracyProgress, ICommand, INotifyPropertyChanged
{
    public CheckAccuracyCommand(TrainerViewModel viewModel) => _viewModel = viewModel;
    private readonly TrainerViewModel _viewModel;
    private float accuracy;
    private float averageScore;
    private int maximum;
    private int current;
    private TestEvaluationState state;
    private string error;
    public float Accuracy
    {
        get => accuracy;
        set
        {
            if (value != accuracy)
            {
                accuracy = value;
                OnPropertyChanged();
            }
        }
    }
    public float AverageScore
    {
        get => averageScore;
        set
        {
            if (value != averageScore)
            {
                averageScore = value;
                OnPropertyChanged();
            }
        }
    }
    public int Maximum
    {
        get => maximum;
        set
        {
            if (value != maximum)
            {
                maximum = value;
                OnPropertyChanged();
            }
        }
    }
    public int Current
    {
        get => current;
        set
        {
            if (value != current)
            {
                current = value;
                OnPropertyChanged();
                if (current >= maximum)
                {
                    State = TestEvaluationState.Evaluating;
                }
            }
        }
    }
    public TestEvaluationState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                OnPropertyChanged();
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
    }

    public string Error
    {
        get => error;
        set
        {
            if (error == value)
            {
                error = value;
                OnPropertyChanged();
            }
        }
    }

    public event EventHandler CanExecuteChanged;
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public bool CanExecute(object parameter)
        => State is TestEvaluationState.Ideal or TestEvaluationState.Evaluated;

    public async void Execute(object parameter)
    {
        Trainer trainer = new();
        FontSetting settings = new(_viewModel.FontSetting);
        EngineType engine = _viewModel.EngineSetting.SelectedEngineType;
        try
        {
            State = TestEvaluationState.Loading;
            await Task.Run(() => trainer.EvaluateAccuracy(settings, engine, this));
            State = TestEvaluationState.Evaluated;
        }
        catch (NotImplementedException)
        {
            Error = $"Engine mode {engine} is not supported";
            State = TestEvaluationState.Error;
        }
        catch (Exception)
        {
            Error = $"Unable to evaluate model, make sure if model is avalable for {engine}";
            State = TestEvaluationState.Error;
        }
        finally
        {
            if (State is not TestEvaluationState.Ideal && State is not TestEvaluationState.Evaluated)
            {
                await Task.Delay(5000);
                State = TestEvaluationState.Ideal;
            }
        }
    }
}
