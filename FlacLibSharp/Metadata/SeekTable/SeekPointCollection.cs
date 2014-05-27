using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp {
    /// <summary>
    /// A set of seekpoints in a seektable in the form of a sorted list, because seektables have to be sorted by Sample Number.
    /// </summary>
    /// <remarks>There is currently an issue with this list: it only kind-of supports multiple placeholders in a weird way!</remarks>
    public class SeekPointCollection : SortedList<UInt64, SeekPoint> {

        private int placeholders;
        /// <summary>
        /// Returns a counter to indicate how many placeholder seekpoints this list has.
        /// These are not actual seekpoints, because they don't really contain information (and their keys are all the same).
        /// </summary>
        /// <remarks>This is a rather special way to support this situation, hopefully in the future this can be changed to something more natural.</remarks>
        public int Placeholders
        {
            get { return this.placeholders; }
            set { this.placeholders = value; }
        }

        /// <summary>
        /// Adds a new seekpoint to the collection.
        /// </summary>
        /// <param name="seekPoint"></param>
        public void Add(SeekPoint seekPoint) {
            if (seekPoint.IsPlaceHolder)
            {
                // Instead of actually adding these to the list, we just count the placeholders.
                this.placeholders += 1;
            }
            else
            {
                base.Add(seekPoint.FirstSampleNumber, seekPoint);
            }
        }

        /// <summary>
        /// Removes a seekpoint from the collection (if found).
        /// </summary>
        /// <param name="seekPoint"></param>
        public void Remove(SeekPoint seekPoint)
        {
            if (seekPoint.IsPlaceHolder)
            {
                this.placeholders -= 1;
            }
            else
            {
                base.Remove(seekPoint.FirstSampleNumber);
            }
        }

    }
}
