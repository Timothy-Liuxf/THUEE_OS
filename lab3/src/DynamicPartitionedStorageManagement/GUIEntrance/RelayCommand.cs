using System;
using System.Windows.Input;

namespace GUIEntrance
{
    public class RelayCommand : ICommand
    {
        private Action execute;
        private Func<bool> canExecute;

        public event EventHandler? CanExecuteChanged
        {
            // To disable never used warning
            add { }
            remove { }
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return canExecute();
        }

        void ICommand.Execute(object? parameter)
        {
            execute();
        }

        public RelayCommand(Action execute)
        {
            this.execute = execute;
            this.canExecute = () => true;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }
}
