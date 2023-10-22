using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Akaha_Gesture {
    class StopCommand : ICommand {
        AkahaGestureModel model;
        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public StopCommand(AkahaGestureModel model) {
            this.model = model;
        }

        public bool CanExecute(object parameter) => model != null && model.isStarted;
        public void Execute(object parameter) {
            model.stop();
        }
    }
}
