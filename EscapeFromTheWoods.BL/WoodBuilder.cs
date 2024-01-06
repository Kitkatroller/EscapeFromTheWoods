using System;
using System.Collections.Generic;
using System.Text;
using EscapeFromTheWoods.BL.Objects;
using EscapeFromTheWoods.BL.Interfaces;

namespace EscapeFromTheWoods.BL
{
    public class WoodBuilder
    {
        //Initialize database
        private IMongoDBWriter dbWriter;
        string bitmapImagesPath;
        string logFilesPath;

        public WoodBuilder(IMongoDBWriter dbWriter, string bitmapImagesPath, string logFilesPath)
        {
            this.dbWriter = dbWriter;
            this.bitmapImagesPath = bitmapImagesPath;
            this.logFilesPath = logFilesPath;
        }

        //Creates a forest "woods" and returns it
        public Wood GetWood(int treeAmount, Map map)
        {
            //Generates trees on uniek spots accros a map of a certain Mapsize with a certain amount of trees
            Random r = new Random(100);
            Dictionary<Tree, int> trees = new Dictionary<Tree, int>();
            IDgenerator.resetTreeID();

            for (int i = 0; i < treeAmount; i++)
            {
                int treeID = IDgenerator.GetTreeID(); ;
                Tree tree;

                do
                {
                    tree = new Tree(treeID, r.Next(map.xmin, map.xmax), r.Next(map.ymin, map.ymax));
                } while (trees.ContainsKey(tree));

                trees.Add(tree, treeID);
            }

            Wood w = new Wood(IDgenerator.GetWoodID(), trees, map, dbWriter, bitmapImagesPath, logFilesPath);
            return w;
        }
    }
}
