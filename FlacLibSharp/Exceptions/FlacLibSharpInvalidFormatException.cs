using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    public class FlacLibSharpInvalidFormatException : FlacLibSharpException
    {
        public string Details { get; set; }

        public FlacLibSharpInvalidFormatException(string details)
            : base("The file is not a valid FLAC file: " + details)
        {
            this.Details = details;
        }
    }
}
