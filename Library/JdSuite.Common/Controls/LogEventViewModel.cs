using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.Common
{
    public class LogEventViewModel
    {


        public LogEventViewModel(LogEventInfo logEventInfo)
        {

            ToolTip = logEventInfo.FormattedMessage;
            Level = logEventInfo.Level.ToString();
            FormattedMessage = logEventInfo.FormattedMessage;
            Exception = logEventInfo.Exception;
            LoggerName = logEventInfo.LoggerName;
            Time = logEventInfo.TimeStamp.ToString(CultureInfo.InvariantCulture);
            
        }


        public string Time { get; private set; }
        public string LoggerName { get; private set; }
        public string Level { get; private set; }
        public string FormattedMessage { get; private set; }
        public Exception Exception { get; private set; }
        public string ToolTip { get; private set; }
        
         
    }
}
