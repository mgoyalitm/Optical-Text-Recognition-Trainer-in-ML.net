using OpenCvSharp;
using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace TrainerApp.Model;

public readonly record struct Alphabet(string FontName, char Character, DrawStyle DrawStyle, double Rotation)
{
    public const int RenderSize = 21;
    public const int CharOffSet = 100;
    public int CharacterId => CharacterMapper.GetCharacterId(Character);
    public Mat Print()
    {
        if (Character == ' ')
        {
            return new Mat(new Size(21, 21), MatType.CV_8UC1, new Scalar(255, 255, 255, 255));
        }

        try
        {
            byte[] data = DrawImage();

            using Mat gray = Cv2.ImDecode(data, ImreadModes.Grayscale);
            using Mat threshold = new();

            Cv2.Threshold(gray, threshold, 200, 255, ThresholdTypes.Binary);

            Rect rect = GetBoundingBox(threshold);

            using Mat cropped = GetCropped(threshold, rect);

            Mat final = new(RenderSize, RenderSize, MatType.CV_8UC1, new Scalar(255));

            int xoffset = (RenderSize - cropped.Width) / 2;
            int yoffset = (RenderSize - cropped.Height) / 2;

            Rect roi = new(xoffset, yoffset, cropped.Width, cropped.Height);
            using Mat region = final[roi];
            cropped.CopyTo(region);

            return final;
        }
        catch (Exception)
        {
            return new Alphabet("Arial", Character, DrawStyle, Rotation).Print();
        }
    }

    private static Rect GetBoundingBox(Mat threshold)
    {
        Cv2.FindContours(threshold, out Point[][] contours, out _, RetrievalModes.List, ContourApproximationModes.ApproxNone);
        return Cv2.BoundingRect(contours.OrderByDescending(x => GetArea(x)).Skip(1).First());
        
        static double GetArea(Point[] contour) => Cv2.ContourArea(contour);
    }

    private static Mat GetCropped(Mat threshold, Rect rect)
    {
        using Mat cropped = new(threshold, rect);
        double ratio = RenderSize / (double)Math.Max(rect.Width, rect.Height);
        Mat output = new();
        Cv2.Resize(cropped, output, new Size(cropped.Width * ratio, cropped.Height * ratio), ratio, ratio, InterpolationFlags.Cubic);
        return output;
    }

    private byte[] DrawImage()
    {

        DrawingVisual visual = new();
        using DrawingContext drawing = visual.RenderOpen();

        System.Windows.FontStyle style = DrawStyle switch
        {
            DrawStyle.Normal or DrawStyle.Bold => System.Windows.FontStyles.Normal,
            _ => System.Windows.FontStyles.Italic,
        };
        System.Windows.FontWeight weight = DrawStyle switch
        {
            DrawStyle.Normal or DrawStyle.Italic => System.Windows.FontWeights.Normal,
            _ => System.Windows.FontWeights.Bold,
        };

        FormattedText text = new(
            textToFormat: $"{Character}",
            culture: CultureInfo.GetCultureInfo("en-us"),
            flowDirection: System.Windows.FlowDirection.LeftToRight,
            typeface: new Typeface(new FontFamily(FontName), style, weight, default), 48, Brushes.Black, 96)
        {
            TextAlignment = System.Windows.TextAlignment.Center,
        };

        drawing.PushTransform(new RotateTransform(Rotation, 48, 48));
        drawing.DrawRectangle(Brushes.White, null, new System.Windows.Rect(0, 0, 96, 96));
        
        drawing.DrawText(text, new System.Windows.Point(48, 24));
        drawing.Close();


        RenderTargetBitmap renderTargetBitmap = new(96, 96, 96, 96, PixelFormats.Pbgra32);
        renderTargetBitmap.Render(visual);

        BmpBitmapEncoder png = new();
        png.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
        using MemoryStream stream = new();
        png.Save(stream);

        return stream.ToArray();
    }
}

//TextBlock textblock = new()
//{
//    Text = $"{Character}",
//    FontSize = 32,
//    FontStyle = DrawStyle switch
//    {
//        DrawStyle.Normal or DrawStyle.Bold => System.Windows.FontStyles.Normal,
//        _ => System.Windows.FontStyles.Italic,
//    },
//    FontWeight = DrawStyle switch
//    {
//        DrawStyle.Normal or DrawStyle.Italic => System.Windows.FontWeights.Normal,
//        _ => System.Windows.FontWeights.Bold,
//    },
//    Width = 64, Height = 64,
//    Foreground = Brushes.Black,
//    LayoutTransform = new RotateTransform(Rotation),
//};
//DrawingVisual visual = new();
//using var render = visual.RenderOpen();
//textblock.RenderSize = new System.Windows.Size(64, 64);
//RenderTargetBitmap renderTargetBitmap = new((int)textblock.ActualWidth, (int)textblock.ActualHeight, 96, 96, PixelFormats.Pbgra32);
//renderTargetBitmap.Render(textblock);

//PngBitmapEncoder png = new();
//png.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
//using MemoryStream stream = new();
//png.Save(stream);

//return stream.ToArray();