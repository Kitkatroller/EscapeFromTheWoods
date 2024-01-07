using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EscapeFromTheWoods.BL.Objects;

namespace EscapeFromTheWoods.BL.Interfaces
{
    public interface IMongoDBWriter
    {
        Task WriteWoodToDBAsync(Tree tree, int woodId);
        Task WriteMonkeyRecordAsync(Monkey monkey, int woodId, int jumpNumber, List<Tree> route);
        Task WriteLogRecordAsync(LogEntry logEntry);
    }
}
