using System.Collections.Generic;

namespace Akaha_Gesture.Stats {
    public class Image {
        public int id { get; set; }
        public string path { get; set; }

        public List<Session> sessions { get; set; }
    }
}
