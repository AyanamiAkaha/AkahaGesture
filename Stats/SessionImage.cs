using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Akaha_Gesture.Stats {
    public class SessionImage {
        [Column("session_id")]
        public DateTime sessionId { get; set; }
        [Column("image_id")]
        public int imageId { get; set; }

        public Image image { get; set; }
        public Session session { get; set; }

        public int order { get; set; }
    }
}