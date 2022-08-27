using System;
using System.Collections.Generic;

namespace Akaha_Gesture {

    public class AkahaGestureModel {
        public List<string> fileNames { get; private set; }

        public AkahaGestureModel() {
            this.fileNames = new List<string>();
        }
    }
}
}
