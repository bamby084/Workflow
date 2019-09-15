using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AppWorkflow.Controls
{
    /// <summary>
	/// A custom header for a <see cref="System.Windows.Controls.TabItem"/>.
	/// </summary>
	public class TabHeader : Grid
    {
        public Button CloseButton;
        public Label NameLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabHeader"/> class.
        /// </summary>
        /// <param name="name">The name of the tab.</param>
        /// <param name="OnClose">Action to take when the tab requests to be closed.</param>
        public TabHeader(string name, RoutedEventHandler OnClose)
        {
            RowDefinitions.Add(new RowDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());

            NameLabel = new Label();
            NameLabel.Content = name;
            Children.Add(NameLabel);
            Grid.SetRow(NameLabel, 0);
            Grid.SetColumn(NameLabel, 0);

            CloseButton = new Button();
            CloseButton.Background = null;
            CloseButton.BorderBrush = null;
            CloseButton.HorizontalAlignment = HorizontalAlignment.Right;
            CloseButton.VerticalAlignment = VerticalAlignment.Top;
            CloseButton.Content = "x";
            CloseButton.FontWeight = FontWeights.Bold;
            CloseButton.FontSize = 14;
            CloseButton.Click += OnClose;
            Children.Add(CloseButton);
            Grid.SetRow(CloseButton, 0);
            Grid.SetColumn(CloseButton, 1);
        }
    }
}
