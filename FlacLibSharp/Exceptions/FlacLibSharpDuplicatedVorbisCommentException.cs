namespace FlacLibSharp.Exceptions
{
    public class FlacLibSharpDuplicatedVorbisCommentException : FlacLibSharpException
    {
        public FlacLibSharpDuplicatedVorbisCommentException(string key)
            : base($"Cannot add vorbis comment with key {key} because a comment with that key already exists. Maybe you can use Update instead?")
        { }
    }
}
