using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {

    /// <summary>
    /// A set of cue sheet track indexes.
    /// </summary>
    public class CueSheetTrackIndexCollection : Collection<CueSheetTrackIndex> {

        // Currently a maximum of 100 index points are allowed
        private const int maxCapacity = 100;

        protected override void InsertItem(int index, CueSheetTrackIndex item)
        {
            if (this.Count >= maxCapacity)
            {
                throw new Exceptions.FlacLibSharpMaxTrackIndicesExceededException(maxCapacity);
            }
            base.InsertItem(index, item);
        }

    }
}
