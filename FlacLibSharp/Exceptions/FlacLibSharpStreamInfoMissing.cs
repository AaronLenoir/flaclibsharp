using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// A Flac file was loaded that didn't contain the stream info. This is the only metadata block that is mandatory.
    /// </summary>
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
