using AppWorkflow.Commands;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

[assembly: DisableDpiAwareness]
namespace AppWorkflow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        // Use MEF to import the modules from the Plugin Directory
        [ImportMany(typeof(IModule), RequiredCreationPolicy = CreationPolicy.NonShared)]
        /// Not for typical use. see <see cref="App.CreateModule"/> to instantiate new modules
        public IEnumerable<ExportFactory<IModule, IDictionary<string, object>>> _modules;

        private static readonly string IPC_PIPE_NAME = "JohnDeiuliis.AppWorkflow.OpenFile";
        private Thread IpcThread = null;
        private bool IsRunning = true;
        NLog.ILogger logger = NLog.LogManager.GetLogger("Application");

        public App()
        {
            logger.Info(".......................Starting application...Release Date 21-Aug-2019 1.0.0.6............................");

            Logger.Initialize("AppWorkflow");

            // Load plugins
            AggregateCatalog catalog = new AggregateCatalog(
                new AssemblyCatalog(
                    System.Reflection.Assembly.GetExecutingAssembly()
                )
            );

            string pluginDir = ConfigurationManager.ConnectionStrings["PluginDirectory"]
                    .ConnectionString;
            string fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), pluginDir);
            foreach (var path in Directory.EnumerateDirectories(fullPath, "*",
                SearchOption.TopDirectoryOnly))
            {
                logger.Trace("including plugin path {0}", path);
                catalog.Catalogs.Add(new DirectoryCatalog(path));
            }

            CompositionContainer pluginContainer = new CompositionContainer(catalog);

            pluginContainer.SatisfyImportsOnce(this);

            Exit += delegate
            {
                IsRunning = false;
                if (IpcThread != null)
                {
                    IpcThread.Join();
                }
            };
        }

        /// <summary>
        /// Creates an instance of the provided module type.
        /// </summary>
        /// <param name="moduleType">Type of the module to instantiate.</param>
        /// <returns>Module instance, if loaded, else null</returns>
        public IModule CreateModule(Type moduleType)
        {
            logger.Info("Creating module object of type {0}", moduleType);
            var factory = _modules.FirstOrDefault(moduleFactory =>
            {
                // TODO: add name metadata for cheaper comparison
                return ((Type)moduleFactory.Metadata["ModuleType"]) == moduleType;
            });

            if (((Type)factory.Metadata["ModuleType"]) != moduleType)
            {
                return null;
            }

            return factory.CreateExport().Value;
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void Application_Startup(object sender, StartupEventArgs e)
        {


            // Allow more than one command line instance
            if (e.Args.Length > 0 && !File.Exists(e.Args[0]))
            {
                new Interpreter(e.Args);
            }
            else
            {
                // Allow only one GUI instance
                using (var mutex = new Mutex(true, "JohnDeiuliis.AppWorkflow.Main", out bool onlyInstance))
                {
                    if (onlyInstance)
                    {
                        IpcThread = new Thread(IpcListener);
                        IpcThread.Start();
                        string filePath = null;
                        if ((e.Args.Length > 0 && File.Exists(e.Args[0])))
                        {
                            filePath = e.Args[0];
                        }
                        new MainWindow(filePath).ShowDialog();
                    }
                    else
                    {  // Pass startup arguments to currently running instance
                        if ((e.Args.Length > 0 && File.Exists(e.Args[0])))
                        {
                            OpenFromFile(e.Args[0], true);
                        }
                        else
                        {
                            // focus process window because exe was double-clicked
                            SetForegroundWindow(GetRemoteProcess().MainWindowHandle);
                        }
                    }
                }
            }

            Shutdown();
        }

        private Process GetRemoteProcess()
        {
            Process current = Process.GetCurrentProcess();
            foreach (Process process in Process.GetProcessesByName(current.ProcessName))
            {
                if (process.Id != current.Id)
                { // Must be the target
                    return process;
                }
            }

            return current;
        }

        private void IpcListener()
        {
            while (IsRunning)
            {
                using (var pipe = new NamedPipeServerStream(IPC_PIPE_NAME))
                {
                    var task = pipe.WaitForConnectionAsync();
                    while (!task.IsCanceled && !task.IsCompleted && !task.IsFaulted)
                    {
                        task.Wait(50);
                        if (!IsRunning)
                        {
                            return;
                        }
                    }

                    if (task.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
                    {
                        pipe.Flush();
                        continue;
                    }

                    string filePath = "";
                    byte[] buffer = new byte[2048];
                    pipe.Read(buffer, 0, 2048);
                    filePath = System.Text.Encoding.Default.GetString(buffer);
                    filePath = filePath.Substring(0, filePath.IndexOf('\0'));

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action(() =>
                        {
                            OpenFromFile(filePath, false);
                        }));
                }
            }
        }

        private void OpenFromFile(string filePath, bool remoteProcess)
        {
            Logger.Log(Severity.DEBUG, LogCategory.APP, filePath);
            if (remoteProcess)
            {
                Process targetProc = GetRemoteProcess();
                using (var pipe = new NamedPipeClientStream(IPC_PIPE_NAME))
                {
                    pipe.Connect();
                    var stream = new MemoryStream();
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(filePath);
                    }
                    pipe.Write(stream.GetBuffer(), 0, stream.GetBuffer().Length);
                }
            }
            else
            {
                ((MainWindow)App.Current.MainWindow).LoadWorkflow(filePath);
                SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
            }
        }




        static void ConfigureNLog()
        {
            var config = new NLog.Config.LoggingConfiguration();

            string fname = "Applicationlog_" + DateTime.Now.ToString("ddMMMyyyy") + ".log";
            var fileName = System.IO.Path.Combine("./", fname);
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = fileName };
            //logfile.Layout = NLog.Layouts.SimpleLayout.FromString(AppWorkflow.Properties.Settings.Default.LogLayout);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            //${date:format=dd-MM-yy HH\:mm\:ss.ff}|${pad:padding=4:fixedLength=true:inner=${level:uppercase=true}}|${threadid:padding=2}|${pad:padCharacter= :padding=18:fixedLength=true:inner=${logger:shortName=false}}.${pad:padCharacter= :padding=-20:fixedLength=true:inner=${callsite:className=false:fileName=false:includeSourcePath=false:methodName=true:cleanNamesOfAnonymousDelegates=True:cleanNamesOfAsyncContinuations=True}}|${message}|${all-event-properties:format=[key]\=[value]:separator=} ${onexception:${newline} Exception_Occured\:${exception:format=toString,Data:maxInnerExceptionLevel=5:innerExceptionSeparator=Inner ${newline}}}
        }
    }
}