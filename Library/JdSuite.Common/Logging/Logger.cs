using JdSuite.Common.Logging.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JdSuite.Common.Logging
{
    public static class Logger
    {
        public const string FILE_MUTEX = "JohnDeiuliis.AppWorkflow.Logger";
        public const string LOG_FILE = "log.txt";
        public static string AppName;
        public static string LogPath = null;
        private static List<Thread> Threads = new List<Thread>();

        public static void Initialize(string appName)
        {
            AppName = appName;
            string rootUserDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(rootUserDir, appName);
            LogPath = Path.Combine(dir, LOG_FILE);

            DateTime startTime = DateTime.Now;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Log(Severity.INFO, LogCategory.APP, "Process start");
        }

        /// <summary>
        /// Logs the specified info to the console and error log.
        /// </summary>
        /// <param name="severity">The log entry severity</param>
        /// <param name="category">Where the log originated from</param>
        /// <param name="description">The description of the logging event</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Log(Severity severity, LogCategory category, string description)
        {
            string output = new LogEntry(AppName, severity, category, description).ToString();

            Console.Out.WriteLineAsync(output);

            Thread thread = new Thread((text) =>
            {
                // Dual-purpose mutex. Prevents multiple threads from accessing
                // the writer and the file at the same time
                using (var mutex = new Mutex(false, FILE_MUTEX))
                {
                    mutex.WaitOne();
                    using (StreamWriter writer = new StreamWriter(File.Open(LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                    {
                        var sanStr = ((string)text).Replace("\r\n", " ").Replace("\n", " ");
                        writer.WriteLine(sanStr);
                    }
                    mutex.ReleaseMutex();
                }
            });

            // Remove done threads
            Threads.RemoveAll(thr => !thr.IsAlive);

            Threads.Add(thread);
            thread.Start(output);
        }

        public static void OnExit()
        {
            Log(Severity.INFO, LogCategory.APP, "Process end");

            // wait for threads to end
            foreach (var thread in Threads)
            {
                thread.Join();
            }
        }
    }
}
