using System;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// This exception is raised when you try to load an invalid flac file.
    /// </summary>
    public class FlacLibSharpSaveNotSupportedException : FlacLibSharpException
    {
        /// <summary>
        /// Technical details on what exactly has gone wrong.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Creates a new exception/
        /// </summary>
        /// <param name="details">Technical details on what exactly has gone wrong.</param>
        public FlacLibSharpSaveNotSupportedException()
            : base("This flac was opened from a stream (not from a filepath) so cannot save the data back, to allow save open the flac from a file and ensure the filestream is seekable.")
        {
            this.Details = String.Empty;
        }
    }
}
