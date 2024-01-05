using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods.BL.Objects
{
    public static class WoodBuilder
    {
        public static Wood GetWood(int size, Map map, string path, MongoDBwriter db, string filePath)
        {
            Random r = new Random(100);
            Dictionary<int, Tree> trees = new Dictionary<int, Tree>();

            for (int i = 0; i < size; i++)
            {
                int treeID = IDgenerator.GetTreeID();
                Tree t = new Tree(treeID, r.Next(map.xmin, map.xmax), r.Next(map.ymin, map.ymax));
                trees.Add(treeID, t);
            }

            Wood w = new Wood(IDgenerator.GetWoodID(), trees, map, path, db, filePath);
            return w;
        }
    }

}
