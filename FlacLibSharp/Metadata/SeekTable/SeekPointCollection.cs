using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp {
    /// <summary>
    /// A set of seekpoints in a seektable in the form of a sorted list.
    /// </summary>
    public class SeekPointCollection : SortedList<UInt64, SeekPoint> {

        /// <summary>
        /// Adds a new seekpoint to the collection.
        /// </summary>
        /// <param name="seekPoint"></param>
        public void Add(SeekPoint seekPoint) {
            base.Add(seekPoint.FirstSampleNumber, seekPoint);
        }
	
    }
}
