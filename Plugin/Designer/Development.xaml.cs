using Designer.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xceed.Wpf.DataGrid;
using Designer.Variables;
using JdSuite.Common.Module;

namespace Designer
{
    /// Interaction logic for Development.xaml
    /// </summary>
    public partial class Development : UserControl
    {
        private const int FAST_PAGEFLIP_NUM = 5;
        private const int ZOOM_AMT = 100;
        private int ActivePageIdx = -1;
        private Records<Record> Records;

        private Field Schema { get; set; }
        private XDocument Data { get; set; }
        private Record CurrentRecord;

        public Development()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the specified pages. If this needs to be called more than once,
        /// it is best to create a new Development to ensure the state is properly
        /// cleared.
        /// </summary>
        /// <param name="pages">The pages.</param>
        /// <param name="xmlSchema">The XML schmea</param>
        /// <param name="xmlData">The XML data.</param>
        public void Load(Pages pages, Field xmlSchema, XDocument xmlData)
        {
            if (pages.Count <= 0)
            {
                return;
            }

            Schema = xmlSchema;
            Data = xmlData;

            bool shouldFilter = ((PagesPropertiesPanel)pages.LayoutProperties).IsPageOrderVariable;

            Records = Processing.Run(pages, Schema, Data, shouldFilter);

            foreach (var record in Records)
            {
                foreach (var page in record)
                {
                    foreach (var control in page.CanvasControls)
                    {
                        control.Border.BorderBrush = null;
                        control.Border.BorderThickness = new Thickness(0);
                        if (control is CanvasRichTextBox tb)
                        {
                            tb.TextBox.BorderBrush = null;
                            tb.TextBox.IsReadOnly = true;
                        }
                    }
                }
            }

            PageNumUpDown.Value = 0;
        }

        private Record GetRecordContainingPage(int index)
        {
            if (index < 0 || index >= Records.MasterPageCounter)
            {
                return null;
            }

            var count = 0;
            for (int rs = 0; rs < Records.Count; rs++)
            {
                var record = Records[rs];
                for (int r = 0; r < record.Count; r++)
                {
                    if (count == index)
                    {
                        return record;
                    }
                    count++;
                }
            }

            return null;
        }

        private PageBase GetPageBy(int index)
        {
            if (index < 0 || index >= Records.MasterPageCounter)
            {
                return null;
            }

            var count = 0;
            for (int rs = 0; rs < Records.Count; rs++)
            {
                var record = Records[rs];
                for (int r = 0; r < record.Count; r++)
                {
                    if (count == index)
                    {
                        return record[r];
                    }
                    count++;
                }
            }

            return null;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            PageNumUpDown.Value = ActivePageIdx - 1;
        }

        private void ButtonFastBack_Click(object sender, RoutedEventArgs e)
        {
            var newIdx = ActivePageIdx - FAST_PAGEFLIP_NUM;
            if (newIdx < 0)
            {
                newIdx = 0;
            }
            PageNumUpDown.Value = newIdx + 1;
        }

        private void ButtonFastForward_Click(object sender, RoutedEventArgs e)
        {
            var newIdx = ActivePageIdx + FAST_PAGEFLIP_NUM;
            if (newIdx >= Records.MasterPageCounter)
            {
                newIdx = Records.MasterPageCounter - 1;
            }
            PageNumUpDown.Value = newIdx + 1;
        }

        private void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            PageNumUpDown.Value = ActivePageIdx + 2; // PageNum == ActivePageIdx + 1
        }

        private void ButtonGotoEnd_Click(object sender, RoutedEventArgs e)
        {
            PageNumUpDown.Value = Records.MasterPageCounter;
        }

        private void ButtonGotoStart_Click(object sender, RoutedEventArgs e)
        {
            PageNumUpDown.Value = 1;
        }

        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Zoom(ZOOM_AMT);
        }

        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Zoom(-ZOOM_AMT);
        }

        private void PageUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (PageNumUpDown.Value.Value <= 0 || !PageNumUpDown.Value.HasValue)
            {
                PageNumUpDown.Value = 1;
                return;
            }

            if (PageNumUpDown.Value.Value > Records.MasterPageCounter)
            {
                PageNumUpDown.Value = Math.Max(1, Records.MasterPageCounter);
                return;
            }

            SetActivePage(PageNumUpDown.Value.Value - 1);
        }

        private void Scroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Zoom(e.Delta);
        }

        private void Scroller_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GetPageBy(ActivePageIdx)?.ScaleToFit(Scroller);
        }

        /// <summary>
        /// Sets the active page.
        /// </summary>
        /// <param name="index">The index from the Pages array to set.</param>
        private void SetActivePage(int index)
        {
            if (index < 0 || index >= Records.MasterPageCounter || index == ActivePageIdx)
            {
                return;
            }

            ActivePageIdx = index;
            if (PageNumUpDown.Value.Value - 1 != ActivePageIdx)
            {
                PageNumUpDown.Value = ActivePageIdx + 1;
            }

            PanelCanvas.Children.Clear();
            var page = GetPageBy(ActivePageIdx);
            PanelCanvas.Children.Add(page.GetRootElement());
            PanelCanvas.InvalidateMeasure();
            PanelCanvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            UpdateNavButtonState();

            var record = GetRecordContainingPage(ActivePageIdx);
            if (record != null && record != CurrentRecord)
            {
                DataView.RefreshData(Schema, Data, record.RecordIndex);
                CurrentRecord = record;
            }
            page.ScaleToFit(Scroller);
        }

        private void UpdateNavButtonState()
        {
            ButtonGotoStart.IsEnabled = true;
            ButtonFastBack.IsEnabled = true;
            ButtonBack.IsEnabled = true;
            ButtonForward.IsEnabled = true;
            ButtonFastForward.IsEnabled = true;
            ButtonGotoEnd.IsEnabled = true;

            if (ActivePageIdx == 0)
            {
                ButtonBack.IsEnabled = false;
                ButtonFastBack.IsEnabled = false;
                ButtonGotoStart.IsEnabled = false;
            }
            if (ActivePageIdx == Records.MasterPageCounter - 1)
            {
                ButtonForward.IsEnabled = false;
                ButtonFastForward.IsEnabled = false;
                ButtonGotoEnd.IsEnabled = false;
            }
        }

        private void Zoom(double delta)
        {
            if (Records.MasterPageCounter == 0)
            {
                return;
            }

            var center = new Point((PanelCanvas.ActualWidth + 20) / 2, (PanelCanvas.ActualHeight + 20) / 2);
            var centerOffset = PanelCanvas.RenderTransform.Transform(center);

            delta *= MainWindow.SCROLL_FACTOR;
            ScaleTransform scaleTransform = GetPageBy(ActivePageIdx).LayoutTransform;
            scaleTransform.ScaleX += delta;
            scaleTransform.ScaleY += delta;
            Scroller.UpdateLayout();

            var newCenterOffset = PanelCanvas.RenderTransform.Transform(center);
            var deltaOffset = newCenterOffset - centerOffset;

            Scroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset + deltaOffset.X);
            Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + deltaOffset.Y);
        }
    }
}