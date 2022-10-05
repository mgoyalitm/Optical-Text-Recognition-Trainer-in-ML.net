namespace TrainerApp.Model;

public interface IProgress
{
    public int Maximum { get; set; }
    public int Current { get; set; }
}

public interface ITrainingProgress : IProgress
{

}

public interface IAccuracyProgress : IProgress
{
    public float Accuracy { get; set; }
    public float AverageScore { get; set; }
}
public interface ITestProgress : IProgress
{
    public bool IsEvaluated { get; set; }
    public double LogLoss { get; set; }
    public double LogLossReduction { get; set; }
    public double MicroAccuracy { get; set; }
    public double MacroAccuracy { get; set; }
}
