using System;

namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// These are the exceptions the FlacLibSharp library produces.
    /// </summary>
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
