using JdSuite.Common.Logging.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.Common.Logging
{
    public class LogEntry
    {
        private const string DATE_TIME_FMT = "yyyy/MM/dd HH:mm:ss";

        public LogEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="application">The application name.</param>
        /// <param name="severity">The log entry severity</param>
        /// <param name="code">Where the log originated from</param>
        /// <param name="description">The description of the logging event</param>
        public LogEntry(string application, Severity severity, LogCategory code, string description)
        {
            Date = DateTime.Now;
            Application = application;
            Severity = severity;
            Code = code;
            Description = description;
        }

        public string Application { get; set; }
        public LogCategory Code { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public Severity Severity { get; set; }

        public static bool TryParse(string str, out LogEntry entry)
        {
            try
            {
                entry = new LogEntry();
                var strs = str.Split(' ');

                DateTime date;
                if (!DateTime.TryParseExact(strs[0] + " " + strs[1], DATE_TIME_FMT, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out date))
                {
                    return false;
                }
                entry.Date = date;

                entry.Application = strs[2];

                Severity severity;
                if (!Enum.TryParse(strs[3], out severity))
                {
                    return false;
                }
                entry.Severity = severity;

                entry.Description = strs[4].Substring(1);
                int descEnd = 0;
                for (int i = 5; i < strs.Length; i++)
                {
                    var s = " " + strs[i];
                    if (s.Contains("\""))
                    {
                        entry.Description += s.Substring(0, s.Length - 1);
                        descEnd = i;
                        break;
                    }

                    entry.Description += s;
                }
                if (descEnd == 0)
                { // description parsing didn't have an end quote
                    return false;
                }

                entry.Code = (LogCategory)int.Parse(strs[descEnd + 1]);
            }
            catch (Exception e)
            {
                entry = null;
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            string output = DateTime.Now.ToString(DATE_TIME_FMT) + " " + Application + " ";

            output += Severity.ToString() + " ";

            output += "\"" + Description + "\" " + (int)Code;
            return output;
        }
    }
}
