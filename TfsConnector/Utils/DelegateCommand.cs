using System;
using System.Windows.Input;

namespace TfsConnector
{
    public class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> canExecute;
        private readonly Action<object> execute;

        public DelegateCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public DelegateCommand(Action<object> execute)
        {
            this.execute = execute;
        }

        public DelegateCommand(Action parameterlessExecute)
        {
            execute = o => parameterlessExecute();
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                return canExecute.Invoke(parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            execute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void InvokeCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Func<T, bool> canExecute;
        private readonly Action<T> execute;

        public DelegateCommand(Func<T, bool> canExecute, Action<T> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }
        public DelegateCommand(Action<T> execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                return canExecute.Invoke((T)parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            execute.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}