namespace FlacLibSharp.Exceptions
{
    /// <summary>
    /// Exception thrown when a padding block of metadata was set to an invalid bit length.
    /// </summary>
    public class FlacLibSharpInvalidPaddingBitCount : FlacLibSharpException
    {
        /// <summary>
        /// Creates a new Exception.
        /// </summary>
        public FlacLibSharpInvalidPaddingBitCount(string details)
            : base(details)
        {
        }
    }
}
