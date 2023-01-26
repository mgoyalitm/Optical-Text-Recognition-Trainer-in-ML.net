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

using CharacterRecognition.Trainer;
using OpenCvSharp;
using System.DirectoryServices;

internal class Program
{
    private const string ModelName = "model";

    static int[] rows = new int[] { 1,2,3,4,5,8,9,10,15,16,17,18,19};
    private static async Task Main()
    {
        foreach (string file in Directory.GetFiles("C:\\Users\\Mahendra\\Desktop\\Fonts", "*.*"))
        {
            string name = Path.GetFileName(file);
            string targetFile = Path.Combine("C:\\Windows\\Fonts", name);
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
                Console.WriteLine(targetFile);
            }
        }



        Console.ReadKey();


        await Task.Yield();
        //CharacterTrainer.TrainImageClassificationData("C:\\Users\\Mahendra\\Desktop\\Fonts All", "Image", rows, false);

        CharacterTrainer.ImageClassificationInterractiveTesting("C:\\Users\\Mahendra\\Desktop\\Fonts All", "Image", rows);

        Console.ReadKey();



        //CharacterTrainer.TrainAndSave("C:\\Users\\Mahendra\\Desktop\\ML.csv", ModelName, true);
        //Console.ReadKey();

        CharacterTrainer.InterractiveTesting("C:\\Users\\Mahendra\\Desktop\\ML.csv", ModelName);



    }
    public static void WriteCsv()
    {
        using StreamWriter writer = new("C:\\Users\\Mahendra\\Desktop\\ML.csv");
        ImageDataBuilder.WriteAsCsv("C:\\Users\\Mahendra\\Desktop\\Fonts All", writer, rows);
        Console.WriteLine("Completed");
    }
    public static async Task CreateFontMaps()
    {

        using StreamWriter writer = new("ML.csv");
        ImageDataBuilder.WriteAsCsv("C:\\Users\\Mahendra\\Desktop\\Fonts All", writer, rows);
        await writer.FlushAsync();
        Directory.CreateDirectory("Fonts");

        Parallel.ForEach(ImageDataBuilder.GetFontNames(), fontname =>
        {
            try
            {
                using Mat fonts = ImageDataBuilder.GetFontMat(fontname);
                fonts.SaveImage($"Fonts\\{fontname}.jpg");
                Console.WriteLine(fontname);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        });
        Console.WriteLine();
        Console.WriteLine("Completed");
    }
}