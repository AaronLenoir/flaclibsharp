using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// These are the exceptions the FlacLibSharp library will produce.
    /// </summary>
    public class FlacLibSharpException : ApplicationException
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
