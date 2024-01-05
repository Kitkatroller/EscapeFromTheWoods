using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods.DL.Records
{
    public class DBLogRecord
    {
        public int LogID { get; set; }
        public int WoodID { get; set; }
        public int MonkeyID { get; set; }
        public string Message { get; set; }

        public DBLogRecord(int logID, int woodID, int monkeyID, string message)
        {
            LogID = logID;
            WoodID = woodID;
            MonkeyID = monkeyID;
            Message = message;
        }
    }

}
