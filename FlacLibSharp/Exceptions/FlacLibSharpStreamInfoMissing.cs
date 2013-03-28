using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    public class FlacLibSharpStreamInfoMissing : FlacLibSharpException
    {
        public FlacLibSharpStreamInfoMissing()
            : base("The FLAC file doesn't contain a StreamInfo metadata block, but this is mandatory.")
        {
        }
    }
}
