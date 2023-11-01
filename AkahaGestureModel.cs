using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using System.Linq;
using Akaha_Gesture.Stats;

namespace Akaha_Gesture
{
    public class AkahaGestureModel : INotifyPropertyChanged {
        public List<string> fileNames { get; private set; }
        private int _secondsPerImage;
        public int secondsPerImage {
            get => _secondsPerImage;
            set {
                _secondsPerImage = value;
                onPropertyChanged("secondsPerImage");
                savePref("secondsPerImage", value);
            }
        }
        private int _imageCount;
        public int imageCount {
            get => _imageCount;
            set {
                _imageCount = value;
                if(value != 0) {
                    onPropertyChanged("imageCount");
                    savePref("imageCount", value);
                }
            }
        }
        private bool _started;
        private bool started {
            get => _started;
            set {
                _started = value;
                onPropertyChanged("isRunning");
                onPropertyChanged("isStarted");
                onPropertyChanged("notStarted");
            }
        }
        private bool _isCountdown = false;
        public bool isCountdown {
            get => _isCountdown;
            private set {
                _isCountdown = value;
                onPropertyChanged("isCountdown");
                onPropertyChanged("isRunning");
            }
        }
        public bool isRunning {
            get => started && !isCountdown;
        }

        private bool _autoMode = true;
        public bool autoMode {
            get => _autoMode;
            set {
                _autoMode = value;
                onPropertyChanged("autoMode");
                onPropertyChanged("manualMode");
                onPropertyChanged("selectTimeEnabled");
                onPropertyChanged("nextPrevEnabled");
                savePref("autoMode", value ? 1 : 0);
            }
        }
        public bool manualMode => !autoMode;
        public bool selectTimeEnabled => !manualMode && !started;
        public bool nextPrevEnabled => manualMode && isRunning;
        public bool notStarted => !started;
        public bool isStarted => started;

        public ICommand selectFilesCommand { get; private set; }
        public ICommand startCommand { get; private set; }
        public ICommand stopCommand { get; private set; }
        public ICommand nextImageCommand { get; private set; }
        public ICommand prevImageCommand { get; private set; }

        public ObservableCollection<string> sessionImages { get; private set; }
        private int? m_currentImageIndex;
        private int? currentImageIndex {
            get => m_currentImageIndex;
            set {
                m_currentImageIndex = value;
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
        DispatcherTimer countdownTimer = null;
        private int m_countdown;
        private int countdown {
            get => m_countdown;
            set {
                m_countdown = value;
                onPropertyChanged("countdownText");
            }
        }

        public string countdownText {
            get {
                return countdown.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        readonly static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AkahaGesture");
        readonly static string lastImagesFilePath = Path.Combine(appDataPath, "lastImages.txt");

        private SessionRepository sessionRepository = new SessionRepository();
        private Session lastSession = null;

        public AkahaGestureModel() {
            this.fileNames = new List<string>();
            this.sessionImages = new ObservableCollection<string>();
            this.selectFilesCommand = new SelectFilesComamnd(this);
            this.startCommand = new StartCommand(this);
            this.stopCommand = new StopCommand(this);
            this.nextImageCommand = new NextImageCommand(this);
            this.prevImageCommand = new PrevImageCommand(this);
            loadPreferences();
        }

        private void savePref(string pref, int value) {
            try {
                var brandKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\AkahaSoftware");
                var key = brandKey.CreateSubKey("AkahaGesture");
                key.SetValue(pref, value, Microsoft.Win32.RegistryValueKind.DWord);
                key.Close();
                brandKey.Close();
            } catch(Exception e) {
                Console.Error.WriteLine(e);
            }
        }

        public void saveLastImages() {
            try {
                Directory.CreateDirectory(appDataPath);
                File.WriteAllLines(lastImagesFilePath, this.fileNames);
            } catch(Exception e) {
                Console.Error.WriteLine(e);
            }
        }

        private void loadPreferences() {
            try {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AkahaSoftware\AkahaGesture");
                this.secondsPerImage = (int)key.GetValue("secondsPerImage", 60);
                this.imageCount = (int)key.GetValue("imageCount", 10);
                this.autoMode = (int)key.GetValue("autoMode", 1) == 1;
                key.Close();
            } catch(Exception e) {
                Console.Error.WriteLine(e);
            }
            string lastImagesFilePath = Path.Combine(appDataPath, "lastImages.txt");
            if (File.Exists(lastImagesFilePath)) {
                string[] lines = File.ReadAllLines(lastImagesFilePath);
                foreach (string line in lines) {
                    if (File.Exists(line)) {
                        this.fileNames.Add(line);
                    }
                }
            }
        }

        public void start() {
            if(this.sessionImages.Count <= 0) return;
            stop();
            this.started = true;
            this.isCountdown = true;
            lastSession = null;
            countdown = 3;
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            countdownTimer.Tick += new EventHandler(onCountdownTick);
            countdownTimer.Start();
        }

        private void onCountdownTick(object sender, EventArgs e) {
            countdown--;
            if (countdownTimer == null) return;
            if(countdown <= 0) {
                isCountdown = false;
                countdownTimer.Stop();
                countdownTimer = null;
                if (started) {
                    this.currentImageIndex = 0;
                    currentImageStarted = DateTime.UtcNow;
                    lastSession = new Session(DateTime.UtcNow, sessionImages.Count, autoMode ? secondsPerImage : 0);
                    lastSession.AddImage(new Image { path = sessionImages[this.currentImageIndex.Value] });
                    if (autoMode) {
                        timer = new DispatcherTimer();
                        timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                        timer.Tick += new EventHandler(onTick);
                        timer.Start();
                    }
                }
            }
        }

        private void onTick(object sender, EventArgs e) {
            Console.WriteLine(currentImageProgress);
            if(currentImageProgress >= 100) {
                this.nextImage();
            }
            this.onPropertyChanged("currentImageProgress");
        }

        public void stop() {
            this.started = false;
            int? lastIdx = this.currentImageIndex;
            this.currentImageIndex = null;
            isCountdown = false;
            countdownTimer?.Stop();
            countdownTimer = null;
            if(timer != null) {
                timer.Stop();
                timer = null;
            }
            if(lastIdx.HasValue) {
                lastSession.end = DateTime.UtcNow;
                sessionRepository.addSession(lastSession);
                showSummary(this.sessionImages.ToList().GetRange(0, lastIdx.Value+1));
            }
        }

        public void nextImage() {
            if(!this.currentImageIndex.HasValue) return;
            if(this.currentImageIndex >= this.sessionImages.Count-1) {
                stop();
            } else {
                this.currentImageStarted = DateTime.UtcNow;
                this.currentImageIndex++;
                lastSession.AddImage(new Image { path = sessionImages[this.currentImageIndex.Value] });
            }
        }

        public void prevImage() {
            if(!this.currentImageIndex.HasValue) return;
            if(this.currentImageIndex > 0) {
                this.currentImageIndex--;
            }
        }

        public void showSummary(List<string> images) {
            var summary = new SummaryWindow(images);
            summary.Show();
        }

        private void onPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
