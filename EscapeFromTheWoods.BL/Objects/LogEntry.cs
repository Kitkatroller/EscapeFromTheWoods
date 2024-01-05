using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods.BL.Objects
{
    public class LogEntry
    {
        public int JumpNumber { get; set; }
        public string MonkeyName { get; set; }
        public int TreeID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"Sprong {JumpNumber}: {MonkeyName} naar boom {TreeID} op locatie ({X}, {Y})";
        }
    }




}
