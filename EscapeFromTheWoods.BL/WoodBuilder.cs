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
        private IMongoDBWriter dBWriter;
        string bitmapImagesPath;
        string logFilesPath;

        public WoodBuilder(IMongoDBWriter dBWriter, string bitmapImagesPath, string logFilesPath)
        {
            this.dBWriter = dBWriter;
            this.bitmapImagesPath = bitmapImagesPath;
            this.logFilesPath = logFilesPath;
        }

        //Creates a forest "woods" and returns it
        public static Wood GetWood(int treeAmount, Map map)
        {
            //Generates trees on uniek spots accros a map of a certain Mapsize and a certain amount of trees
            Random r = new Random(100);
            Dictionary<int, Tree> trees = new Dictionary<int, Tree>();

            for (int i = 0; i < treeAmount; i++)
            {
                int treeID = IDgenerator.GetTreeID(); ;
                Tree tree;

                do
                {
                    tree = new Tree(treeID, r.Next(map.xmin, map.xmax), r.Next(map.ymin, map.ymax));
                } while (trees.Values.Any(t => t.x == tree.x && t.y == tree.y));

                trees.Add(treeID, tree);
            }

            Wood w = new Wood(IDgenerator.GetWoodID(), trees, map, dBWriter);
            return w;
        }
    }

}
