using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// This exception is raised when too many index points are added to a CueSheet Track.
    /// </summary>
    public class FlacLibSharpMaxTrackIndicesExceededException : FlacLibSharpException
    {

        /// <summary>
        /// Creates an exception to indicate that too many index points have been added to a CueSheet Track.
        /// </summary>
        /// <param name="maxIndexPoints">How many index points that are allowed.</param>
        public FlacLibSharpMaxTrackIndicesExceededException(int maxIndexPoints)
            : base(String.Format("A cuesheet track can have no more than {0} index points.", maxIndexPoints))
        {
        }
    }
}
