using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// This exception is raised when you try to load an invalid flac file.
    /// </summary>
    [Serializable]
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
            : base(string.Format("The file is not a valid FLAC file: {0}", details))
        {
            this.Details = details;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("Details", this.Details);
        }

    }
}
