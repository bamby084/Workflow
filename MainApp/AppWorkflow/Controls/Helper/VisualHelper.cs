using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppWorkflow.Controls
{
    public static class VisualHelper
    {
        public static T FindParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentDepObj = child;
            do
            {
                parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
                T parent = parentDepObj as T;
                if (parent != null) return parent;
            }
            while (parentDepObj != null);
            return null;
        }

        public static Rect GetBoundingBox(FrameworkElement element)
        {
            return element.RenderTransform.TransformBounds(new Rect(element.RenderSize));
        }

        public static ScaleTransform GetImageScaleWithBounds(double maxSize, BitmapImage image)
        {
            

            double constrainedWidth;
            double constrainedHeight;
            double widthAspect = image.PixelWidth / (double)image.PixelHeight;
            double heightAspect = image.PixelHeight / (double)image.PixelWidth;

            if (image.PixelWidth > image.PixelHeight)
            {
                constrainedWidth = maxSize;
                constrainedHeight = maxSize * heightAspect;
            }
            else
            {
                constrainedWidth = maxSize * widthAspect;
                constrainedHeight = maxSize;
            }

            var t= new ScaleTransform(
                    constrainedWidth / image.PixelWidth,
                    constrainedHeight / image.PixelHeight
                );

            return t;
        }

        public static ScaleTransform GetImageScaleWithBounds(Rect size, BitmapImage image)
        {


            double constrainedWidth;
            double constrainedHeight;
            double widthAspect = image.PixelWidth / (double)image.PixelHeight;
            double heightAspect = image.PixelHeight / (double)image.PixelWidth;

            if (image.PixelWidth > image.PixelHeight)
            {
                constrainedWidth = size.Width;
                constrainedHeight = size.Width * heightAspect;
            }
            else
            {
                constrainedWidth = size.Width * widthAspect;
                constrainedHeight = size.Height;
            }

            var t = new ScaleTransform(
                    constrainedWidth / image.PixelWidth,
                    constrainedHeight / image.PixelHeight
                );

            return t;
        }

        public static bool IsMouseOver(UIElement element, UIElement reference)
        {
            return IsPointOver(Mouse.GetPosition(reference), element, reference);
        }

        public static bool IsPointOver(Point point, UIElement element, Visual reference)
        {
            bool isOver = false;
            VisualTreeHelper.HitTest(Window.GetWindow(reference), null, (HitTestResult result) =>
            {
                if (result.VisualHit == element)
                {
                    isOver = true;
                    return HitTestResultBehavior.Stop;
                }
                return HitTestResultBehavior.Continue;
            }, new PointHitTestParameters(point));

            return isOver;
        }

        public static void RemoveChild(this DependencyObject parent, UIElement child)
        {
            var panel = parent as Panel;
            if (panel != null)
            {
                panel.Children.Remove(child);
                return;
            }

            var decorator = parent as Decorator;
            if (decorator != null)
            {
                if (decorator.Child == child)
                {
                    decorator.Child = null;
                }
                return;
            }

            var contentPresenter = parent as ContentPresenter;
            if (contentPresenter != null)
            {
                if (contentPresenter.Content == child)
                {
                    contentPresenter.Content = null;
                }
                return;
            }

            var contentControl = parent as ContentControl;
            if (contentControl != null)
            {
                if (contentControl.Content == child)
                {
                    contentControl.Content = null;
                }
                return;
            }

            // maybe more
        }
    }
}
