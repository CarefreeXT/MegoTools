using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Caredev.MegoTools.ViewModel
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _ExecuteAction = execute;
            _CanExecuteAction = canExecute;
        }

        private readonly Action<object> _ExecuteAction;
        private readonly Func<object, bool> _CanExecuteAction;

        public bool CanExecute(object parameter)
        {
            return _CanExecuteAction == null || _CanExecuteAction(parameter);
        }

        public void Execute(object parameter)
        {
            if (_CanExecuteAction == null || _CanExecuteAction(parameter))
            {
                _ExecuteAction(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
