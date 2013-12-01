using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// This exception is raised when you try to load an invalid flac file.
    /// </summary>
    public class FlacLibSharpInvalidFormatException : FlacLibSharpException
    {
        /// <summary>
        /// Technical details on what exactly has gone wrong.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Creates a new exception/
        /// </summary>
        /// <param name="details">Technical details on what exactly has gone wrong.</param>
        public FlacLibSharpInvalidFormatException(string details)
            : base("The file is not a valid FLAC file: " + details)
        {
            this.Details = details;
        }
    }
}
