using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using EscapeFromTheWoods.BL.Interfaces;
using EscapeFromTheWoods.BL.Records;
using System.Diagnostics;

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
            this.monkeys = new List<Monkey>();//Instantiates empty list of monkeys, user adds the monkeys in the program
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
                //Gets a random tree key from trees
                selectedTree = trees.Keys.ElementAt(r.Next(trees.Count));
            }
            while (selectedTree.hasMonkey); //Checks if the chosen tree already has a monkey, if so search again for different tree

            Monkey m = new Monkey(monkeyID, monkeyName, selectedTree);

            monkeys.Add(m);

            selectedTree.hasMonkey = true;
        }
        //Determines the quikest route the monkey takes to get out of the forest
        public List<Tree> EscapeMonkey(Monkey monkey)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start escape from wood with Id: {woodID},{monkey.name}");

            Dictionary<int, bool> visited = new Dictionary<int, bool>();
            List<Tree> treeList = new List<Tree>(trees.Keys);

            foreach (var tree in treeList)
            {
                visited[tree.treeID] = false;
            }

            List<Tree> route = new List<Tree>() { monkey.tree };

            while (true)
            {
                visited[monkey.tree.treeID] = true;

                Tree closestTree = null;
                double minDistanceSq = double.MaxValue;

                foreach (Tree t in treeList)
                {
                    if (!visited[t.treeID] && !t.hasMonkey)
                    {
                        double dSq = (t.x - monkey.tree.x) * (t.x - monkey.tree.x) +
                                     (t.y - monkey.tree.y) * (t.y - monkey.tree.y);
                        if (dSq < minDistanceSq)
                        {
                            minDistanceSq = dSq;
                            closestTree = t;
                        }
                    }
                }

                double distanceToBorder = (new List<double>(){ map.ymax - monkey.tree.y,
            map.xmax - monkey.tree.x,monkey.tree.y - map.ymin,monkey.tree.x - map.xmin }).Min();

                if (closestTree == null || distanceToBorder < Math.Sqrt(minDistanceSq))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                route.Add(closestTree);
                monkey.tree = closestTree;
            }
        }

        //Adds the escape route for the different monkeys to a list routes
        public async Task Escape()
        {
            try
            {
                List<List<Tree>> routes = new List<List<Tree>>();
                foreach (Monkey m in monkeys)
                {
                    List<Tree> route = EscapeMonkey(m);
                    routes.Add(route);
                    await WriteRouteToDBAsync(m, route);
                }
                await WriteEscaperoutesToBitmapAsync(routes);
                await WriteLogToFileAsync(logFilesPath, logEntries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Escape: {ex.Message}");
            }
        }


        //Datalayer related methods
        //Database
        public async Task WriteWoodToDBAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} start");

            var semaphore = new SemaphoreSlim(5);

            var tasks = new List<Task>();
            foreach (var tree in trees.Keys)
            {
                await semaphore.WaitAsync();
                tasks.Add(dBWriter.WriteWoodToDBAsync(tree, woodID).ContinueWith(t => semaphore.Release()));
            }

            await Task.WhenAll(tasks);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} end");
        }
        private async Task WriteRouteToDBAsync(Monkey monkey, List<Tree> route)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");

            List<Task> logWriteTasks = new List<Task>();

            for (int jumpNumber = 0; jumpNumber < route.Count; jumpNumber++)
            {
                await dBWriter.WriteMonkeyRecordAsync(monkey, woodID, jumpNumber, route);
               
                LogEntry logEntry = new LogEntry
                {
                    JumpNumber = jumpNumber,
                    MonkeyName = monkey.name,
                    MonkeyId = monkey.monkeyID,
                    TreeID = route[jumpNumber].treeID,
                    WoodId = this.woodID,
                    X = route[jumpNumber].x,
                    Y = route[jumpNumber].y
                };
                                
                var logWriteTask = dBWriter.WriteLogRecordAsync(logEntry);
                logWriteTasks.Add(logWriteTask);
                logEntries.Add(logEntry);
            }

            // Wait for all log write tasks to complete
            await Task.WhenAll(logWriteTasks);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }

        //Bitmap images
        private async Task WriteEscaperoutesToBitmapAsync(List<List<Tree>> routes)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");

            Color[] cvalues = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            using (Bitmap bm = new Bitmap((map.xmax - map.xmin) * drawingFactor, (map.ymax - map.ymin) * drawingFactor))
            using (Graphics g = Graphics.FromImage(bm))
            {
                int delta = drawingFactor / 2;
                Pen p = new Pen(Color.Green, 1);

                foreach (var tree in trees)
                {
                    Tree t = tree.Key;
                    g.DrawEllipse(p, t.x * drawingFactor, t.y * drawingFactor, drawingFactor, drawingFactor);
                }

                int colorN = 0;
                foreach (List<Tree> route in routes)
                {
                    int p1x = route[0].x * drawingFactor + delta;
                    int p1y = route[0].y * drawingFactor + delta;
                    Color color = cvalues[colorN % cvalues.Length];
                    using (Pen pen = new Pen(color, 1))
                    {
                        g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                        g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                        for (int i = 1; i < route.Count; i++)
                        {
                            g.DrawLine(pen, p1x, p1y, route[i].x * drawingFactor + delta, route[i].y * drawingFactor + delta);
                            p1x = route[i].x * drawingFactor + delta;
                            p1y = route[i].y * drawingFactor + delta;
                        }
                    }
                    colorN++;
                }

                string filePath = Path.Combine(bitmapImagesPath, woodID.ToString() + "_escapeRoutes.jpg");
                using (var memoryStream = new MemoryStream())
                {
                    bm.Save(memoryStream, ImageFormat.Jpeg);
                    memoryStream.Position = 0;
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                    {
                        await memoryStream.CopyToAsync(fileStream);
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} end");
        }

        //LogFiles
        private async Task WriteLogToFileAsync(string filePath, List<LogEntry> logEntries)
        {
            string fileName = "log" + woodID + ".txt";
            string fullPath = Path.Combine(filePath, fileName);

            var sortedLogEntries = logEntries.OrderBy(entry => entry.JumpNumber).ThenBy(entry => entry.MonkeyName);

            using (StreamWriter file = new StreamWriter(fullPath))
            {
                foreach (var entry in sortedLogEntries)
                {
                    await file.WriteLineAsync(entry.ToString());
                }
            }
        }

    }
}
