using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Metadata {
    public class SeekPointCollection : SortedList<UInt64, SeekPoint> {

        public void Add(SeekPoint seekPoint) {
            base.Add(seekPoint.FirstSampleNumber, seekPoint);
        }
	
    }
}
