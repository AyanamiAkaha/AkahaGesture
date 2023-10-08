using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Akaha_Gesture {
    public class AkahaGestureModel : INotifyPropertyChanged {
        public ObservableCollection<string> fileNames { get; private set; }
        public int secondsPerImage { get; set; }
        public int imageCount { get; set; }
        private bool _started;
        private bool started {
            get => _started;
            set {
                _started = value;
                onPropertyChanged("isStarted");
                onPropertyChanged("notStarted");
            }
        }

        private bool _autoMode = true;
        public bool autoMode {
            get => _autoMode;
            set {
                _autoMode = value;
                onPropertyChanged("manualMode");
                onPropertyChanged("selectTimeEnabled");
                onPropertyChanged("nextPrevEnabled");
            }
        }
        public bool manualMode => !autoMode;
        public bool selectTimeEnabled => !manualMode && !started;
        public bool nextPrevEnabled => manualMode && started;
        public bool notStarted => !started;
        public bool isStarted => started;

        public ICommand selectFilesCommand { get; private set; }
        public ICommand startCommand { get; private set; }
        public ICommand stopCommand { get; private set; }
        public ICommand nextImage { get; private set; }
        public ICommand prevImage { get; private set; }

        public ObservableCollection<string> sessionImages { get; private set; }
        private int? _currentImageIndex;
        private int? currentImageIndex {
            get => _currentImageIndex;
            set {
                _currentImageIndex = value;
                onPropertyChanged("currentImage");
                onPropertyChanged("currentImageStr");
            }
        }
        public string currentImageStr {
            get {
                return string.Format("{0} / {1}", this.currentImageIndex + 1, this.imageCount);
            }
        }
        public string currentImage {
            get {
                if (currentImageIndex.HasValue) {
                    return sessionImages[currentImageIndex.Value];
                }
                return "";
            }
        }
        private DateTime currentImageStarted;
        public double currentImageProgress {
            get {
                return (DateTime.UtcNow.Subtract(currentImageStarted).TotalSeconds / secondsPerImage) * 100.0;
            }
        }
        DispatcherTimer timer = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public AkahaGestureModel() {
            this.fileNames = new ObservableCollection<string>();
            this.sessionImages = new ObservableCollection<string>();
            this.secondsPerImage = 60;
            this.imageCount = 10;

            this.selectFilesCommand = new SelectFilesComamnd(this);
            this.startCommand = new StartCommand(this);
            this.stopCommand = new StopCommand(this);
            this.nextImage = new NextImageCommand(this);
            this.prevImage = new PrevImageCommand(this);
        }

        public void Start() {
            if(this.sessionImages.Count <= 0) return;
            Stop();
            this.currentImageStarted = DateTime.UtcNow;
            this.currentImageIndex = 0;
            this.started = true;
            if (autoMode) {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                timer.Tick += new EventHandler(onTick);
                timer.Start();
            }
        }

        private void onTick(object sender, EventArgs e) {
            Console.WriteLine(currentImageProgress);
            if(currentImageProgress >= 100) {
                this.NextImage();
            }
            this.onPropertyChanged("currentImageProgress");
        }

        public void Stop() {
            this.started = false;
            this.currentImageIndex = null;
            if(timer != null) {
                timer.Stop();
                timer = null;
            }
        }
        public void NextImage() {
            if(!this.currentImageIndex.HasValue) return;
            if(this.currentImageIndex >= this.sessionImages.Count-1) {
                Stop();
            } else {
                this.currentImageStarted = DateTime.UtcNow;
                this.currentImageIndex++;
            }
        }

        public void PrevImage() {
            if(!this.currentImageIndex.HasValue) return;
            if(this.currentImageIndex > 0) {
                this.currentImageIndex--;
            }
        }

        private void onPropertyChanged(string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
