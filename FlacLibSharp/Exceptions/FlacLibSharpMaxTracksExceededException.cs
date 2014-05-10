using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// This exception is raised when too many tracks are added to a CueSheet.
    /// </summary>
    public class FlacLibSharpMaxTracksExceededException : FlacLibSharpException
    {

        /// <summary>
        /// Creates a new exception indicating there are too many tracks added to a CueSheet.
        /// </summary>
        /// <param name="maxTracks">How many tracks that are allowed.</param>
        public FlacLibSharpMaxTracksExceededException(int maxTracks)
            : base(String.Format("A cuesheet can have no more than {0} tracks.", maxTracks))
        {
        }
    }
}
