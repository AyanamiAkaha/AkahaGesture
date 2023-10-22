using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace Akaha_Gesture
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SummaryWindow : Window
    {
        public SummaryWindow(List<string> images)
        {
            InitializeComponent();
            SummaryWindowModel model = (SummaryWindowModel)DataContext;
            foreach(string img in images) {
                model.sessionImages.Add(img);
            }
            var maxHeight = SystemParameters.WorkArea.Height;
            var maxWidth = SystemParameters.WorkArea.Width;
            var desiredWidth = maxHeight * 0.707;
            var desiredHeight = maxWidth / 0.707;
            if(desiredWidth > maxWidth) {
                Width = maxWidth;
                Height = desiredHeight;
            } else {
                Width = desiredWidth;
                Height = maxHeight;
            }
            ResizeMode = ResizeMode.NoResize;
            // Aiming at 2 rows of 5 images on an A4 paper, which is my typical gesture drawing session
            // if there's more, they'll overflow to next rows
            model.imgMaxWidth = (Width - 2*model.pageMargin - 4*model.imageMarginX) / 5;
            model.imgMaxHeight = (Height - 2*model.pageMargin - model.imageMarginY) / 2;
        }

        private void closeClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
