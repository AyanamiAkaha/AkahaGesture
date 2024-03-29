﻿using Akaha_Gesture.Stats;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Akaha_Gesture {
    public class SummaryWindowModel : INotifyPropertyChanged {

        private SessionRepository sessionRepository = new SessionRepository();

        public ObservableCollection<string> sessionImages { get; set; }
        private double m_imgMaxWidth;
        public double imgMaxWidth {
            get => m_imgMaxWidth;
            set {
                m_imgMaxWidth = value;
                onPropertyChanged("imgMaxWidth");
            }
        }
        private double m_imgMaxHeight;
        public double imgMaxHeight {
            get => m_imgMaxHeight;
            set {
                m_imgMaxHeight = value;
                onPropertyChanged("imgMaxHeight");
            }
        }
        private double m_pageMargin;
        public double pageMargin {
            get => m_pageMargin;
            set {
                m_pageMargin = value;
                onPropertyChanged("pageMargin");
            }
        }
        private double m_imageMarginX;
        public double imageMarginX {
            get => m_imageMarginX;
            set {
                m_imageMarginX = value;
                onPropertyChanged("imageMarginX");
                onPropertyChanged("margins");
            }
        }
        private double m_imageMarginY;
        public double imageMarginY {
            get => m_imageMarginY;
            set {
                m_imageMarginY = value;
                onPropertyChanged("imageMarginY");
                onPropertyChanged("margins");
            }
        }

        public Thickness margins {
            get => new Thickness(imageMarginX, imageMarginY, imageMarginX, imageMarginY);
        }

        private Session m_session;
        public Session lastSession {
            get {
                if (m_session == null) {
                    m_session = sessionRepository.getLastSession();
                }
                return m_session;
            }
        }

        public string sessionStats {
            get => string.Format("Session {0} - {1} of {2} images in {3:hh\\:mm\\:ss}", lastSession.start, lastSession.images.Count, lastSession.numImages, lastSession.duration);
        }

        public SummaryWindowModel() {
            sessionImages = new ObservableCollection<string>();
            imgMaxWidth = 100;
            imgMaxHeight = 100;
            pageMargin = 50;
            imageMarginX = 20;
            imageMarginY = 10;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void onPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
