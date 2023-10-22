using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Akaha_Gesture {
    public class SummaryWindowModel {

        public ObservableCollection<string> sessionImages { get; set; }
        public double imgMaxWidth { get; set; }
        public double imgMaxHeight { get; set; }
        public double pageMargin { get; private set; }
        public double imageMarginX { get; private set; }
        public double imageMarginY { get; private set; }

        public Thickness margins {
            get => new Thickness(imageMarginY, imageMarginX, imageMarginY, imageMarginX);
        }

        public SummaryWindowModel() {
            sessionImages = new ObservableCollection<string>();
            imgMaxWidth = 100;
            imgMaxHeight = 100;
            pageMargin = 100;
            imageMarginX = 50;
            imageMarginY = 50;
        }
    }
}
