﻿using System;
using System.Windows.Input;

namespace Designer
{
    public class RelayCommand : ICommand
    {
        private readonly bool _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute, bool canExecute = true)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
