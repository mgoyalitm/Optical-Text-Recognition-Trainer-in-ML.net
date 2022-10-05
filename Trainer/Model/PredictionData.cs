using Microsoft.ML.Data;

namespace TrainerApp.Model;

public class PredictionData
{
    [ColumnName("PredictedLabel")]
    public int CharacterId { get; set; }
    [ColumnName("Score")]
    public float[] Score { get; set; }
}