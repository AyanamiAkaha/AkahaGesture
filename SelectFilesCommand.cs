using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Akaha_Gesture {
    class SelectFilesComamnd : ICommand {
        private AkahaGestureModel model;
        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public SelectFilesComamnd(AkahaGestureModel model) { this.model = model; }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = "Select image files";
            dialog.Filter = "Images (*.png, *.jpg)|*.png;*.jpg";
            bool? result = dialog.ShowDialog();
            if(result == true) {
                model.fileNames.Clear();
                foreach(var f in  dialog.FileNames) {
                    model.fileNames.Add(f);
                }
            }
        }
    }
}
