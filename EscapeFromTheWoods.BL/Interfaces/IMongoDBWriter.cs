using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EscapeFromTheWoods.BL.Objects;
using EscapeFromTheWoods.BL.Records;

namespace EscapeFromTheWoods.BL.Interfaces
{
    public interface IMongoDBWriter
    {
        Task WriteWoodToDBAsync(Tree tree, int woodId);
        void WriteMonkeyRecords(List<DBMonkeyRecord> data);
        void WriteLogRecord(LogEntry logEntry);
    }
}
