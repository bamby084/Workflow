using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Designer.DesignerTools
{
    public class DesignerToolBar: Control
    {
        public ObservableCollection<DesignerTool> Tools { get; set; }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(DesignerToolBar), new PropertyMetadata(default(Orientation)));

        public static readonly DependencyProperty SelectedToolProperty = DependencyProperty.Register(
            "SelectedTool", typeof(DesignerTool), typeof(DesignerToolBar), new PropertyMetadata(default(DesignerTool)));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(IEnumerable<DesignerTool>), typeof(DesignerToolBar),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        public Orientation Orientation
        {
            get => (Orientation) GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public DesignerTool SelectedTool
        {
            get { return (DesignerTool) GetValue(SelectedToolProperty); }
            set { SetValue(SelectedToolProperty, value); }
        }

        public IEnumerable<DesignerTool> ItemsSource
        {
            get => (IEnumerable<DesignerTool>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        static DesignerToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerToolBar),
                new FrameworkPropertyMetadata(typeof(DesignerToolBar)));
        }

        public DesignerToolBar()
        {
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var toolbar = (DesignerToolBar)sender;
                toolbar.HandleItemsSourceChanged(e);
            }
        }

        private void HandleItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null)
            {
                foreach(DesignerTool tool in (IEnumerable<DesignerTool>)e.NewValue)
                {
                    tool.PropertyChanged += OnItemPropertyChanged;
                }
            }

            if(e.OldValue != null)
            {
                foreach (DesignerTool tool in (IEnumerable<DesignerTool>)e.OldValue)
                {
                    tool.PropertyChanged -= OnItemPropertyChanged;
                }
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(DesignerTool.IsSelected))
            {
                var tool = (DesignerTool)sender;
                if (tool.IsSelected)
                {
                    SelectedTool = tool;
                }
            }
        }
    }
}