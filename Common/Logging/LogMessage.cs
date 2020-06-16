using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging
{
    public class LogMessage
    {
        public string EventSource { get; set; }
        public string UserId { get; set; }
        public string CustomMessage { get; set; }
        public Exception Exception { get; set; }
    }
}
