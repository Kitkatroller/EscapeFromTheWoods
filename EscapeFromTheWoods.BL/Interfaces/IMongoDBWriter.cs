using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EscapeFromTheWoods.BL.Records;

namespace EscapeFromTheWoods.BL.Interfaces
{
    public interface IMongoDBWriter
    {
        void WriteMonkeyRecords(List<DBMonkeyRecord> data);
        void WriteWoodRecords(List<DBWoodRecord> data);
        void WriteLogRecords(List<DBLogRecord> data);
    }
}
