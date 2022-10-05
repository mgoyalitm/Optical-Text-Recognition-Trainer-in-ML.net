using Microsoft.ML.Data;
using OpenCvSharp;

namespace TrainerApp.Model;

public class CharacterData
{
    const int Size = Alphabet.RenderSize * Alphabet.RenderSize; 
    [LoadColumn(0)]
    public int CharacterId { get; set; }
    [LoadColumn(1, Size), VectorType(Size)]
    public float[] Pixels { get; set; }
}

public class CharacterOpticalData
{
    [LoadColumn(0)]
    public int CharacterId { get; set; }
    public byte[] Pixels { get; set; }
}

