/*
 MIT License

Copyright (c) 2023 Mahendra Goyal

Permission is hereby granted, free of charge, to any person obtaining 
a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
OTHER DEALINGS IN THE SOFTWARE.
 */

using Microsoft.ML.Data;
using OpenCvSharp;
using System.Drawing;
using Tensorflow;

namespace TrainerApp.Model;

public class CharacterData
{
    [LoadColumn(0)]
    public int CharacterId { get; set; }
    [LoadColumn(1, 64), VectorType(64)]
    public float[] Pixels { get; set; }

    public static float[] GetData(byte[] data)
    {
        float[] result = new float[64];
        int total = 0;
        for (int x = 0; x < 21; x++)
        {
            for (int y = 0; y < 21; y++)
            {
                if (data[(y * 21) + x] < 240)
                {
                    total++;
          
                    int x1 = x / 3;
                    int y1 = y / 3;
                    result[(y1 * 7) + x1] += 1;

                    int x2 = x / 7;
                    int y2 = y / 7;
                    result[49 + (y2 * 3) + x2] += 1;
                    
                    
                    int x3 = x > 10 ? 1 : 0;
                    int y3 = y > 10 ? 1 : 0;

                    if (x != 10 && y != 10)
                    {
                        result[58 + (y3 * 2) + x3] += 1;
                    }
                }

            }
        }

        for (int i = 0; i < 49; i++)
        {
            int val = (int)(result[i] + 1) / 3;
            result[i] = MathF.Ceiling(val);
        }

        for (int i = 49; i < 58; i++)
        {
            int val = (int)result[i] / 5;
            result[i] = MathF.Ceiling(val);
        }

        for (int i = 58; i < 62; i++)
        {
            result[i] = MathF.Ceiling(100 * result[i] / total);
        }

        result[62] = total;
        result[63] = 441 - total;

        return result;
    }
}

public class CharacterOpticalData
{
    [LoadColumn(0)]
    public int CharacterId { get; set; }
    public byte[] Pixels { get; set; }
}

