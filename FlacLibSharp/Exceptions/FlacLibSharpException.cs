using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// These are the exceptions the FlacLibSharp library will produce.
    /// </summary>
    [Serializable]
    public class FlacLibSharpException : Exception
    {

        /// <summary>
        /// Creates a new FlacLibSharp exception.
        /// </summary>
        /// <param name="message">A clear description of the issue.</param>
        public FlacLibSharpException(string message) : base(message)
        {

        }
    }
}
