using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// A padding block of metadata was set to an invalid bit length.
    /// </summary>
    [Serializable]
    public class FlacLibSharpInvalidPaddingBitCount : FlacLibSharpException
    {
        /// <summary>
        /// Creates a new FlacLibSharpStreamInfoMissing.
        /// </summary>
        public FlacLibSharpInvalidPaddingBitCount(string details)
            : base(details)
        {
        }
    }
}
