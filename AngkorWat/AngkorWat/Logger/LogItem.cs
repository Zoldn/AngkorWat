using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Logger
{
    public class LogItem
    {
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public int RequestClassId { get; set; } = 0;
        public int ResponseClassId { get; set; } = 0;
        public int ReturnCode { get; set; } = 0;
        public string RequestString { get; set; } = string.Empty;
        public string ResponseString { get; set; } = string.Empty;
        public LogItem() { }
    }
}
