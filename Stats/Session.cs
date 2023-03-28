using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Akaha_Gesture.Stats {
    public class Session {
        [Key]
        public DateTime start { get; private set; }
        [Column("n_images")]
        public int numImages { get; private set; }
        [Column("sec_per_image")]
        public int secondsPerImage { get; private set; }
        public string summary { get; private set; }
        [Column("results")]
        private string m_Results;
        internal virtual List<SessionImage> sessionImages { get; set; }

        [NotMapped]
        public List<string> results {
            get => m_Results.Split(';').ToList();
            private set {
                m_Results = string.Join(";", value);
            }
        }

        [NotMapped]
        public List<string> images {
            get {
                return (
                    from si in sessionImages 
                    orderby si.order 
                    select si.image.path
                ).ToList();
            }
        }

        private Session() { }

        public Session(DateTime start, int numImages, int secondsPerImage) {
            this.start = start;
            this.numImages = numImages;
            this.secondsPerImage = secondsPerImage;
            this.m_Results = null;
            this.sessionImages = null;
        }

        public void AddImage(Image img) {
            if(this.sessionImages == null) {
                this.sessionImages = new List<SessionImage>();
            }
            this.sessionImages.Add(new SessionImage { session = this, image = img, order = sessionImages.Count() });
        }

        public override string ToString()
        {
            return $"Session {{ Start: {start}, NumImages: {numImages}, SecondsPerImage: {secondsPerImage}, Summary: {summary}, Results: {m_Results}, Images: {string.Join(", ", images)} }}";
        }

    }

}
