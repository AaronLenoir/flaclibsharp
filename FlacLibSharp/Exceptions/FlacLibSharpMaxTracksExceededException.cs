using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// This exception is raised when you try to load an invalid flac file.
    /// </summary>
    public class FlacLibSharpMaxTracksExceededException : FlacLibSharpException
    {

        /// <summary>
        /// Creates a new exception/
        /// </summary>
        /// <param name="details">Technical details on what exactly has gone wrong.</param>
        public FlacLibSharpMaxTracksExceededException(int maxTracks)
            : base(String.Format("A cuesheet can have no more than {0} tracks.", maxTracks))
        {
        }
    }
}
