using EscapeFromTheWoods.MongoDB;
using System;
using System.Diagnostics;
using EscapeFromTheWoods.BL;
using EscapeFromTheWoods.BL.Objects;

namespace EscapeFromTheWoods.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initialize Database and woodBuilder
            string connectionString = @"mongodb://localhost:27017";
            MongoDBWriter dbWriter = new MongoDBWriter(connectionString);
            WoodBuilder wBuilder = new WoodBuilder(dbWriter, bitmapImagesPath, logFilesPath);

            //Paths to where the user wishes to store the bitmap and logFiles
            string bitmapImagesPath = @"C:\Users\phili\Documents\Hogent-Semester3\Programmeren\OpdrachtRefactoring\EscapeFromTheWoodsToRefactor\EscapeFromTheWoodsToRefactor\BitmapImages";
            string logFilesPath = @"C:\Users\phili\Documents\Hogent-Semester3\Programmeren\OpdrachtRefactoring\EscapeFromTheWoodsToRefactor\EscapeFromTheWoodsToRefactor\Files";







            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Hello World!");
    
            Map m1 = new Map(0, 500, 0, 500);
            Wood w1 = WoodBuilder.GetWood(500, m1);
            w1.PlaceMonkey("Alice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Janice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Toby", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Mindy", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Jos", IDgenerator.GetMonkeyID());
            
            Map m2 = new Map(0, 200, 0, 400);
            Wood w2 = WoodBuilder.GetWood(2500, m2);
            w2.PlaceMonkey("Tom", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jerry", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Tiffany", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Mozes", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jebus", IDgenerator.GetMonkeyID());

            Map m3 = new Map(0, 400, 0, 400);
            Wood w3 = WoodBuilder.GetWood(2000, m3);
            w3.PlaceMonkey("Kelly", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kenji", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kobe", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kendra", IDgenerator.GetMonkeyID());

            w1.WriteWoodToDB();
            w2.WriteWoodToDB();
            w3.WriteWoodToDB();
            w1.Escape();
            w2.Escape();
            w3.Escape();
            
            stopwatch.Stop();
            // Write result.
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");
        }
    }
}
