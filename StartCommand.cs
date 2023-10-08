using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Akaha_Gesture {
    class StartCommand : ICommand {
        public static Random random = new Random();

        AkahaGestureModel model;
        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public StartCommand(AkahaGestureModel model) {
            this.model = model;
        }

        public bool CanExecute(object parameter) => model != null && !model.isStarted;
        public void Execute(object parameter) {
            model.sessionImages.Clear();
            var remaining = new List<string>(model.fileNames);
            int wantedCount = Math.Min(model.imageCount, model.fileNames.Count);
            while(model.sessionImages.Count < wantedCount ) {
                int idx = random.Next(0, remaining.Count);
                string s = remaining[idx];
                remaining.RemoveAt(idx);
                model.sessionImages.Add(s);
            }
            model.Start();
        }
    }
}
