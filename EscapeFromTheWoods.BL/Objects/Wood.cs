using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using EscapeFromTheWoods.BL.Interfaces;
using EscapeFromTheWoods.BL.Records;

namespace EscapeFromTheWoods.BL.Objects
{
    public class Wood
    {
        //Database variables
        IMongoDBWriter dBWriter;
        string bitmapImagesPath;
        string logFilesPath;
        List<LogEntry> logEntries = new List<LogEntry>();

        //Variables
        private const int drawingFactor = 8;
        public int woodID { get; set; }
        private Random r = new Random(1);
        public Dictionary<Tree, int> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;        
        

        //Constructor
        public Wood(int woodID, Dictionary<Tree, int> trees, Map map, IMongoDBWriter dBWriter, string bitmapImagesPath, string logFilesPath)
        {
            this.woodID = woodID;
            this.trees = trees;
            this.monkeys = new List<Monkey>();//Instatiates empty list of monkeys, user adds the monkeys in the program
            this.map = map;

            //Database
            this.dBWriter = dBWriter;
            this.bitmapImagesPath = bitmapImagesPath;
            this.logFilesPath = logFilesPath;
        }

        //Methods
        //Places a new monkey in a random tree and adds it to the list of monkeys for this wood"forest"
        public void PlaceMonkey(string monkeyName, int monkeyID)
        {
            Tree selectedTree = null;
            do
            {
                // Get a random tree key from the dictionary
                selectedTree = trees.Keys.ElementAt(r.Next(trees.Count));
            }
            while (selectedTree.hasMonkey); // checks if the chosen tree already has a monkey, if so search again for different tree

            Monkey m = new Monkey(monkeyID, monkeyName, selectedTree);

            monkeys.Add(m);

            selectedTree.hasMonkey = true;
        }
        //Determines the route the monkey takes to get out of the forest
        public List<Tree> EscapeMonkey(Monkey monkey)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start {woodID},{monkey.name}");

            //Keeps track of the trees a monkey has visited
            Dictionary<int, bool> visited = new Dictionary<int, bool>();

            //Populating the Dictionary with keys from the collection trees
            foreach (var tree in trees)
            {
                visited.Add(tree.Value, false); // Gebruik de Key van de KeyValuePair
            }

            List<Tree> route = new List<Tree>() { monkey.tree };

            do
            {
                visited[monkey.tree.treeID] = true;

                SortedList<double, List<Tree>> distanceToMonkey = new SortedList<double, List<Tree>>();

                foreach (var tree in trees) // Itereer over KeyValuePair
                {
                    Tree t = tree.Key; // Krijg de Tree uit de KeyValuePair
                    if (!visited[t.treeID] && !t.hasMonkey)//check if its already visited or contains a monkey
                    {
                        double d = Math.Sqrt(Math.Pow(t.x - monkey.tree.x, 2) + Math.Pow(t.y - monkey.tree.y, 2));
                        if (!distanceToMonkey.ContainsKey(d))
                        {
                            distanceToMonkey[d] = new List<Tree>();
                        }
                        distanceToMonkey[d].Add(t);
                    }
                }

                //distance to border            
                //noord oost zuid west
                double distanceToBorder = (new List<double>(){ map.ymax - monkey.tree.y,
                map.xmax - monkey.tree.x,monkey.tree.y-map.ymin,monkey.tree.x-map.xmin }).Min();
                
                if (distanceToMonkey.Count == 0)
                {
                    writeRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }
                if (distanceToBorder < distanceToMonkey.First().Key)
                {
                    writeRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();
                
                WriteLogToFile(logFilesPath, logEntries);
            }
            while (true);
        }
        //Adds the escape route for the different monkeys to a list routes
        public void Escape()
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (Monkey m in monkeys)
            {
                routes.Add(EscapeMonkey(m));
            }
            WriteEscaperoutesToBitmap(routes);
        }

        //Datalayer related methods
        //Database
        private void writeRouteToDB(Monkey monkey, List<Tree> route)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");

            List<DBMonkeyRecord> records = new List<DBMonkeyRecord>();
            int jumpNumber = 1;
            for (int j = 0; j < route.Count; j++)
            {
                records.Add(new DBMonkeyRecord(monkey.monkeyID, monkey.name, woodID, j, route[j].treeID, route[j].x, route[j].y));
                
                logEntries.Add(new LogEntry
                {
                    JumpNumber = jumpNumber,
                    MonkeyName = monkey.name,
                    MonkeyId = monkey.monkeyID,
                    TreeID = route[j].treeID,
                    X = route[j].x,
                    Y = route[j].y
                });
                jumpNumber++;
            }
            writeLogsToDB(logEntries);

            dBWriter.WriteMonkeyRecords(records);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }
        public void WriteWoodToDB()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} start");

            List<DBWoodRecord> records = new List<DBWoodRecord>();
            foreach (var tree in trees)
            {
                Tree t = tree.Key; 
                records.Add(new DBWoodRecord(woodID, t.treeID, t.x, t.y));
            }

            dBWriter.WriteWoodRecords(records);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} end");
        }
        private void writeLogsToDB(List<LogEntry> logEntries)
        {
            ////Console.ForegroundColor = ConsoleColor.Blue;
            ////Console.WriteLine($"{woodID}:write db logs for {monkey.name} start");

            List<DBLogRecord> dbLogRecords = new List<DBLogRecord>();

            foreach (var logEntry in logEntries)
            {
                string message = $"Monkey {logEntry.MonkeyName} moved to tree {logEntry.TreeID} at location ({logEntry.X}, {logEntry.Y})";
                dbLogRecords.Add(new DBLogRecord(
                    IDgenerator.GetLogID(),
                    this.woodID,
                    logEntry.MonkeyId,
                    message
                ));
            }

            dBWriter.WriteLogRecords(dbLogRecords);

            ////Console.ForegroundColor = ConsoleColor.Blue;
            ////Console.WriteLine($"{woodID}:write db logs for {monkey.name} end");
        }

        //Bitmap images
        public void WriteEscaperoutesToBitmap(List<List<Tree>> routes)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");

            Color[] cvalues = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            Bitmap bm = new Bitmap((map.xmax - map.xmin) * drawingFactor, (map.ymax - map.ymin) * drawingFactor);
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);

            foreach (var tree in trees)
            {
                Tree t = tree.Key; // Krijg de Tree uit de KeyValuePair
                g.DrawEllipse(p, t.x * drawingFactor, t.y * drawingFactor, drawingFactor, drawingFactor);
            }

            int colorN = 0;
            foreach (List<Tree> route in routes)
            {
                int p1x = route[0].x * drawingFactor + delta;
                int p1y = route[0].y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                for (int i = 1; i < route.Count; i++)
                {
                    g.DrawLine(pen, p1x, p1y, route[i].x * drawingFactor + delta, route[i].y * drawingFactor + delta);
                    p1x = route[i].x * drawingFactor + delta;
                    p1y = route[i].y * drawingFactor + delta;
                }
                colorN++;
            }
            bm.Save(Path.Combine(bitmapImagesPath, woodID.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} end");
        }

        //LogFiles
        public void WriteLogToFile(string filePath, List<LogEntry> logEntries)
        {
            string fileName = "log" + woodID + ".txt";
            string fullPath = Path.Combine(filePath, fileName);

            var sortedLogEntries = logEntries.OrderBy(entry => entry.JumpNumber).ThenBy(entry => entry.MonkeyName);


            using (StreamWriter file = new StreamWriter(fullPath))
            {
                foreach (var entry in sortedLogEntries)
                {
                    file.WriteLine(entry.ToString());
                }
            }
        }
    }
}
