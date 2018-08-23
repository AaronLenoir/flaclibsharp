using System;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// Exception raised when a Flac file was loaded that didn't contain the stream info.
    /// </summary>
    /// <remarks>Stream Info is the only metadata block that is mandatory.</remarks>
    public class FlacLibSharpStreamInfoMissing : FlacLibSharpException
    {
        /// <summary>
        /// Creates a new FlacLibSharpStreamInfoMissing.
        /// </summary>
        public FlacLibSharpStreamInfoMissing()
            : base("The FLAC file doesn't contain a StreamInfo metadata block, but this is mandatory.")
        {
        }
    }
}
