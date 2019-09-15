using NLog;
using NLog.Common;
using NLog.LogReceiverService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JdSuite.Common.Controls
{
    /// <summary>
    /// Interaction logic for LoggingControl.xaml
    /// </summary>
    public partial class LoggingControl : UserControl
    {
        public event EventHandler ItemAdded = delegate { };

        public ObservableCollection<LogEventViewModel> LogEntries { get; private set; }
        public bool IsTargetConfigured { get; private set; }

        public int MaxRowCount { get; set; } = 10000;

        public bool AutoScrollToLast { get; set; } = true;

        public LoggingControl()
        {
            IsTargetConfigured = false;
            LogEntries = new ObservableCollection<LogEventViewModel>();

            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (NlogViewerTarget target in LogManager.Configuration.AllTargets.Where(t => t is NlogViewerTarget).Cast<NlogViewerTarget>())
                {
                    IsTargetConfigured = true;
                    target.LogReceived += LogReceived;
                }
            }

            this.dataGrid.ItemsSource = LogEntries;
        }

        public void Log(LogEventViewModel logEvent)
        {
            this.LogEntries.Add(logEvent);
        }



        protected void LogReceived(AsyncLogEventInfo log)
        {
            LogEventViewModel vm = new LogEventViewModel(log.LogEvent);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MaxRowCount > 0 && LogEntries.Count >= MaxRowCount)
                    LogEntries.RemoveAt(0);
                LogEntries.Add(vm);
                if (AutoScrollToLast) ScrollToLast();
                //ItemAdded(this, (NLogEvent)log.LogEvent);
            }));
        }
        public void Clear()
        {
            LogEntries.Clear();
        }

        public void ScrollToFirst()
        {
            if (this.LogEntries.Count <= 0) return;

            this.dataGrid.SelectedIndex = 0;
            ScrollToItem(this.dataGrid.SelectedItem);
        }
        public void ScrollToLast()
        {
            if (this.LogEntries.Count <= 0) return;

            this.dataGrid.SelectedIndex = this.LogEntries.Count - 1;
            ScrollToItem(this.dataGrid.SelectedItem);
        }

        private void ScrollToItem(object item)
        {
            this.dataGrid.ScrollIntoView(item);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
