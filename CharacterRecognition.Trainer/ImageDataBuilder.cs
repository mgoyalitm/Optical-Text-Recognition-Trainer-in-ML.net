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


using OpenCvSharp;
using System;
using System.Data;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows.Shapes;

namespace CharacterRecognition.Trainer;

public static class ImageDataBuilder
{
    public const int Size = 32;
    public const string AlphaNumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static IEnumerable<OpticalCharacterData> GetImageData(string path, int[] rows)
    {
        foreach (string file in Directory.GetFiles(path, "*.jpg").OrderBy(x => Random.Shared.Next()))
        {
            using Mat glyphs = Cv2.ImRead(file, ImreadModes.Grayscale);
            using Mat threshold = new();
            Cv2.Threshold(glyphs, threshold, 200, 255, ThresholdTypes.Binary);

            for (int charIndex = 0; charIndex < AlphaNumeric.Length; charIndex++)
            {
                int cordX = charIndex * Size;
                char character = AlphaNumeric[charIndex];

                for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                {
                    int cordY = rowIndex * Size;

                    Rect roi = new(cordX, cordY, Size, Size);
                    using Mat charMat = threshold[roi];
                    using Mat charMatResized = new();
                    Cv2.Resize(charMat, charMatResized, new OpenCvSharp.Size(16, 16), 0.5d, 0.5d, InterpolationFlags.LinearExact);
                    using Mat charThreshold = new();
                    Cv2.Threshold(charMatResized, charThreshold, 200, 255, ThresholdTypes.Binary);

                    byte[] data = charMat.ToBytes();

                    yield return new OpticalCharacterData
                    {
                        Character = (character - '0'),
                        PixelValues = data,
                    };
                }
            }
        }
    }
    public static void WriteAsCsv(string path, StreamWriter writer, int[] rows)
    {
        foreach (string file in Directory.GetFiles(path, "*.jpg"))
        {
            using Mat glyphs = Cv2.ImRead(file, ImreadModes.Grayscale);
            using Mat threshold = new();
            Cv2.Threshold(glyphs, threshold, 200, 255, ThresholdTypes.Binary);

            for (int charIndex = 0; charIndex < AlphaNumeric.Length; charIndex++)
            {
                int cordX = charIndex * Size;
                char character = AlphaNumeric[charIndex];

                for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                {
                    int cordY = rowIndex * Size;

                    Rect roi = new(cordX, cordY, Size, Size);
                    using Mat charMat = threshold[roi];
                    using Mat charMatResized = new();
                    Cv2.Resize(charMat, charMatResized, new OpenCvSharp.Size(16, 16), 0.5d, 0.5d, InterpolationFlags.LinearExact);
                    using Mat charThreshold = new();
                    Cv2.Threshold(charMatResized, charThreshold, 200, 255, ThresholdTypes.Binary);

                    bool success = charMat.GetArray(out byte[] charBytes);

                    if (success == true)
                    {
                        writer.Write($"{(int)(character - '0')},");

                        for (int i = 0; i < charBytes.Length; i++)
                        {
                            byte bit = charBytes[i];
                            if (bit == 0 || bit == 255)
                            {
                                writer.Write(bit == 255 ? "0" : "1");
                                if (i != charBytes.Length - 1)
                                {
                                    writer.Write(',');
                                }
                            }
                        }

                        writer.WriteLine();
                    }
                }
            }
        }
    }
    public static Mat GetFontMat(string fontname)
    {
        FontStyle[] styles = new FontStyle[] { FontStyle.Regular, FontStyle.Italic, FontStyle.Bold };
        int[] angles = new int[] { -15, -10, -5, 0, 5, 10, 15 };
        char[] chars = AlphaNumeric.ToCharArray();

        int width = chars.Length * Size;
        int height = styles.Length * angles.Length * Size;
        Mat fontMat = new(height, width, MatType.CV_8UC1, new Scalar(255));

        for (int charIndex = 0; charIndex < chars.Length; charIndex++)
        {
            for (int styleIndex = 0; styleIndex < styles.Length; styleIndex++)
            {
                for (int angleIndex = 0; angleIndex < angles.Length; angleIndex++)
                {
                    char character = chars[charIndex];
                    FontStyle style = styles[styleIndex];
                    int angle = angles[angleIndex];

                    int y = (angles.Length * styleIndex + angleIndex) * Size;
                    int x = charIndex * Size;

                    Rect roi = new(x, y, Size, Size);

                    using Mat region = fontMat[roi];
                    using Mat charMat = GetCharactorMat(character, fontname, style, angle);

                    charMat.CopyTo(region);
                }
            }
        }


        return fontMat;
    }
    public static Mat GetCharactorMat(char character, string fontName, FontStyle style, int rotation)
    {
        using Mat gray = Mat.FromImageData(PrintCharactor(character, fontName, style, rotation), ImreadModes.Grayscale);
        using Mat threshold = new();
        Cv2.Threshold(gray, threshold, 200, 255, ThresholdTypes.Binary);

        Rect rect = GetBoundingBox(threshold);

        using Mat cropped = GetCropped(threshold, rect);

        Mat final = new(Size, Size, MatType.CV_8UC1, new Scalar(255));

        int xoffset = (Size - cropped.Width) / 2;
        int yoffset = (Size - cropped.Height) / 2;

        Rect roi = new(xoffset, yoffset, cropped.Width, cropped.Height);
        using Mat region = final[roi];
        cropped.CopyTo(region);

        return final;
    }

    private static Mat GetCropped(Mat threshold, Rect rect)
    {
        using Mat cropped = new(threshold, rect);
        double ratio = Size / (double)Math.Max(rect.Width, rect.Height);
        Mat output = new();
        Cv2.Resize(cropped, output, new OpenCvSharp.Size(cropped.Width * ratio, cropped.Height * ratio), ratio, ratio, InterpolationFlags.Cubic);
        return output;
    }

    private static Rect GetBoundingBox(Mat threshold)
    {
        Cv2.FindContours(threshold, out OpenCvSharp.Point[][] contours, out _, RetrievalModes.List, ContourApproximationModes.ApproxTC89KCOS);
        return Cv2.BoundingRect(contours.OrderByDescending(x => GetArea(x)).Skip(1).First());
        static double GetArea(OpenCvSharp.Point[] contour) => Cv2.ContourArea(contour);
    }



    public static byte[] PrintCharactor(char character, string fontName, FontStyle style, int rotation)
    {
        using Font font = new(fontName, 48, style);
        using Bitmap characterBitmap = new(144, 144);
        using Graphics characterGraphics = Graphics.FromImage(characterBitmap);

        characterGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        characterGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        characterGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        characterGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

        characterGraphics.RotateTransform(rotation);
        characterGraphics.Clear(Color.White);

        System.Drawing.Point offset = GetOffset(rotation);
        characterGraphics.DrawString(character.ToString(), font, new SolidBrush(Color.Black), offset.X, offset.Y);
        characterGraphics.Flush();
        using MemoryStream stream = new();

        characterBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

        return stream.ToArray();
    }

    private static System.Drawing.Point GetOffset(int rotation)
    {
        System.Drawing.Point point = new(72, 72);
        System.Drawing.Drawing2D.Matrix matrix = new();
        matrix.Rotate(-rotation);
        System.Drawing.Point[] offsets = new System.Drawing.Point[1] { point };
        matrix.TransformPoints(offsets);
        return new System.Drawing.Point(offsets[0].X - 36, offsets[0].Y - 36);
    }

    public static IEnumerable<string> GetFontNames(string culture = "en-us")
    {
        foreach (System.Windows.Media.FontFamily fontfamily in System.Windows.Media.Fonts.SystemFontFamilies)
        {
            foreach (string font in fontfamily.FamilyNames
                .Where(x => x.Key.ToString().Trim().ToLower() == culture)
                .Select(x => x.Value).Distinct())
            {
                yield return font;
            }
        }
    }


}