using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppWorkflow.Controls
{
    public class DragCanvas : Canvas
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(DragCanvas));
        private CanvasModule DragModule;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //logger.Info("Entered");
            base.OnMouseMove(e);

            if (DragModule == null)
            {
                return;
            }

            Point mousePos = e.GetPosition(this);
            Canvas.SetLeft(DragModule, mousePos.X);
            Canvas.SetTop(DragModule, mousePos.Y);
           // logger.Info("Leaving");
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            //logger.Info("Entered");
            base.OnMouseUp(e);

            var window = (MainWindow)Window.GetWindow(this);
            if (DragModule == null)
            {
                return;
            }

            CanvasModule module = DragModule;
            Children.Remove(DragModule);

            if (window.TabControl.SelectedItem == null)
            {
                return;
            }

            // Modules are only allowed to be dropped on the workflow canvas
            // So if it's not being dropped there, let it fall out of scope
            var tabController = (TabControl)window.FindName("TabControl");
            var wfCanvas = ((WorkflowScrollViewer)tabController.SelectedContent).ChildCanvas;

            if (VisualHelper.IsMouseOver(wfCanvas, this))
            {
                wfCanvas.Children.Add(module);
            }
            //logger.Info("Leaving");
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (visualAdded != null && visualAdded is CanvasModule)
            {
                Visual visual = visualAdded as Visual;
                OnAddCanvasModule((CanvasModule)visual);
            }

            if (visualRemoved != null && visualRemoved is CanvasModule)
            {
                Visual visual = visualRemoved as Visual;
                OnRemoveCanvasModule((CanvasModule)visual);
            }
        }

        private void OnAddCanvasModule(CanvasModule module)
        {
            DragModule = module;
            logger.Trace($"Setting DragModule={module.Module.DisplayName}");
            //logger.Info("DragModule={0}", module.DisplayName.Text);
        }

        private void OnRemoveCanvasModule(CanvasModule module)
        {
            logger.Info("DragModule=null");
            DragModule = null;
        }
    }
}
