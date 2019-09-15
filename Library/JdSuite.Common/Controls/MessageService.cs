using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JdSuite.Common
{
    public static class MessageService
    {
        public static MessageBoxResult ShowInfo(string Title, string Message)
        {
            return MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult ShowError(string Title, string Message)
        {
            return MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowQuestion(string Title, string Message)
        {
            return MessageBox.Show(Message, Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }
    }
}
