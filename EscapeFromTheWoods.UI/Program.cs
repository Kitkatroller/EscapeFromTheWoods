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
            //Paths to where the user wishes to store the bitmap and logFiles
            string bitmapImagesPath = @"C:\Users\phili\Documents\Hogent-Semester3\Programmeren\OpdrachtRefactoring\EscapeFromTheWoods\EscapeFromTheWoods.DL\BitMapImages\";
            string logFilesPath = @"C:\Users\phili\Documents\Hogent-Semester3\Programmeren\OpdrachtRefactoring\EscapeFromTheWoods\EscapeFromTheWoods.DL\LogFiles\";
            MongoDBWriter dbWriter = new MongoDBWriter(connectionString);
            WoodBuilder wBuilder = new WoodBuilder(dbWriter, bitmapImagesPath, logFilesPath);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Map m1 = new Map(0, 500, 0, 500);
            Wood w1 = wBuilder.GetWood(500, m1);
            w1.PlaceMonkey("Alice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Janice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Toby", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Mindy", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Jos", IDgenerator.GetMonkeyID());

            Map m2 = new Map(0, 200, 0, 400);
            Wood w2 = wBuilder.GetWood(2500, m2);
            w2.PlaceMonkey("Tom", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jerry", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Tiffany", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Mozes", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jebus", IDgenerator.GetMonkeyID());

            Map m3 = new Map(0, 1000, 0, 1000);
            Wood w3 = wBuilder.GetWood(20000, m3);
            w3.PlaceMonkey("Kelly", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kenji", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kobe", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kendra", IDgenerator.GetMonkeyID());
            OutputElapsedTimeInRed(stopwatch); //Time to create woods and place monkeys

            //Times to write wood to database
            w1.WriteWoodToDB();
            OutputElapsedTimeInRed(stopwatch);
            w2.WriteWoodToDB();
            OutputElapsedTimeInRed(stopwatch);
            w3.WriteWoodToDB();
            OutputElapsedTimeInRed(stopwatch);

            //times to calculate and write escape routes to db
            w1.Escape();
            OutputElapsedTimeInRed(stopwatch);
            w2.Escape();
            OutputElapsedTimeInRed(stopwatch);
            w3.Escape();
            OutputElapsedTimeInRed(stopwatch);

            stopwatch.Stop();

            // Write result.
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");

        }

        public static void OutputElapsedTimeInRed(Stopwatch stopwatch)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
        }
    }
}
