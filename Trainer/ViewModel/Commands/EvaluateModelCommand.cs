namespace TrainerApp.ViewModel.Commands;

public enum TestEvaluationState { Ideal, Loading, Evaluating, Evaluated, Error }

public class EvaluateModelCommand : ITestProgress, ICommand, INotifyPropertyChanged
{
    public EvaluateModelCommand(TrainerViewModel viewModel) => _viewModel = viewModel;
    private readonly TrainerViewModel _viewModel;
    private bool isEvaluated;
    private double logLoss;
    private double logLossReduction;
    private double microAccuracy;
    private double macroAccuracy;
    private int maximum;
    private int current;
    private TestEvaluationState state;
    private string error;

    public bool IsEvaluated
    {
        get => isEvaluated;
        set
        {
            if (isEvaluated != value)
            {
                isEvaluated = value;
                OnPropertyChanged();
                State = TestEvaluationState.Evaluated;
            }
        }
    }
    public double LogLoss
    {
        get => logLoss;
        set
        {
            if (logLoss != value)
            {
                logLoss = value;
                OnPropertyChanged();
            }
        }
    }
    public double LogLossReduction
    {
        get => logLossReduction;
        set
        {
            if (logLossReduction != value)
            {
                logLossReduction = value;
                OnPropertyChanged();
            }
        }
    }
    public double MicroAccuracy
    {
        get => microAccuracy;
        set
        {
            if (microAccuracy != value)
            {
                microAccuracy = value;
                OnPropertyChanged();
            }
        }
    }
    public double MacroAccuracy
    {
        get => macroAccuracy;
        set
        {
            if (macroAccuracy != value)
            {
                macroAccuracy = value;
                OnPropertyChanged();
            }
        }
    }
    public int Maximum
    {
        get => maximum;
        set
        {
            if (maximum != value)
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
            if (current != value)
            {
                current = value;
                OnPropertyChanged();
                if (current == maximum)
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

    public bool CanExecute(object _) 
        => State is TestEvaluationState.Ideal or TestEvaluationState.Evaluated;

    public async void Execute(object _)
    {

        Trainer trainer = new();
        FontSetting settings = new(_viewModel.FontSetting);
        EngineType engine = _viewModel.EngineSetting.SelectedEngineType;
        try
        {
            State = TestEvaluationState.Loading;
            await Task.Run(() => trainer.Evaluate(settings, engine, this));
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
