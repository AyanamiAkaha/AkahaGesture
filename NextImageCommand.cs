using System;
using System.Windows.Input;

namespace Akaha_Gesture
{
    internal class NextImageCommand : ICommand
    {
        AkahaGestureModel model;
        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public NextImageCommand(AkahaGestureModel model) {
            this.model = model;
        }

        public bool CanExecute(object parameter) => model != null && model.manualMode && model.isStarted;
        public void Execute(object parameter) {
            model.NextImage();
        }
    }
}