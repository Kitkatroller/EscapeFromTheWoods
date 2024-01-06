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
        void WriteMonkeyRecords(List<DBMonkeyRecord> data);
        void WriteWoodRecord(DBWoodRecord record);
        void WriteLogRecords(List<DBLogRecord> data);
        void WriteLogRecord(DBLogRecord data);
        void WriteLogRecord(LogEntry logEntry);
    }
}
