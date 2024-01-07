using EscapeFromTheWoods.BL.Objects;
using EscapeFromTheWoods.DL.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.DL.Mappers
{
    public static class MonkeyRecordMapper
    {
        public static DBMonkeyRecord MapToDBMonkeyRecord(Monkey monkey, int woodId, int jumpNumber, List<Tree> route)
        {            
            return new DBMonkeyRecord(monkey.monkeyID,
                monkey.name,
                woodId,
                jumpNumber,
                route[jumpNumber].treeID,
                route[jumpNumber].x,
                route[jumpNumber].y
                );
        }
    }
}
