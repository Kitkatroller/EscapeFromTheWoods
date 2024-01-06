using EscapeFromTheWoods.BL.Objects;
using EscapeFromTheWoods.BL.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.DL.Mappers
{
    public static class LogRecordMapper
    {
        public static DBLogRecord MapToDBLogRecord(LogEntry logEntry)
        {
            string message = $"Monkey {logEntry.MonkeyName} moved to tree {logEntry.TreeID} at location ({logEntry.X}, {logEntry.Y})";
            return new DBLogRecord(
                IDgenerator.GetLogID(),
                logEntry.WoodId,
                logEntry.MonkeyId,
                message
            );
        }
    }
}
