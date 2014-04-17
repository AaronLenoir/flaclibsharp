using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A set of Cue Sheet Tracks
    /// </summary>
    public class CueSheetTrackCollection : Collection<CueSheetTrack> {

        // Currently a maximum of 100 tracks are allowed (99 regular tracks and 1 lead-out track)
        private const int maxCapacity = 100;

        protected override void InsertItem(int index, CueSheetTrack item)
        {
            if (this.Count >= maxCapacity)
            {
                throw new Exceptions.FlacLibSharpMaxTracksExceededException(maxCapacity);
            }
            base.InsertItem(index, item);
        }
    }
}
