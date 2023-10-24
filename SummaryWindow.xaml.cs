using System.Collections.Generic;
using System.Windows;

namespace Akaha_Gesture
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SummaryWindow : Window
    {
        public SummaryWindow(List<string> images)
        {
            var hBorder = 2*SystemParameters.FixedFrameVerticalBorderWidth + 50;
            var vBorder = SystemParameters.CaptionHeight + SystemParameters.FixedFrameHorizontalBorderHeight + 50;
            var maxHeight = SystemParameters.WorkArea.Height - vBorder;
            var maxWidth = SystemParameters.WorkArea.Width - hBorder;
            var desiredWidth = maxHeight / 0.707;
            var desiredHeight = maxWidth * 0.707;
            Top = 40;
            if(desiredWidth > maxWidth) {
                Width = maxWidth;
                Height = desiredHeight + hBorder;
            } else {
                Width = desiredWidth + vBorder;
                Height = maxHeight;
            }
            ResizeMode = ResizeMode.NoResize;
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            SummaryWindowModel model = (SummaryWindowModel)DataContext;
            foreach(string img in images) {
                model.sessionImages.Add(img);
            }
            // Aiming at 2 rows of 5 images on an A4 paper, which is my typical gesture drawing session
            // if there's more, they'll overflow to next rows
            double scrollWidth = images.Count > 10 ? SystemParameters.VerticalScrollBarWidth : 0;
            const double closePanelHeight = 100;
            model.imgMaxWidth = (Width - 2*model.pageMargin - 10*model.imageMarginX - hBorder - scrollWidth)/5;
            model.imgMaxHeight = (Height - 2*model.pageMargin - 4*model.imageMarginY - vBorder - closePanelHeight)/2;
        }

        private void closeClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
