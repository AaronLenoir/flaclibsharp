using System.Collections.Generic;

namespace FlacLibSharp
{
    /// <summary>
    /// Wrapper class for easy access to some of the Flac functions
    /// </summary>
    public class FastFlac
    {

        #region Metadata Fetchers

        /// <summary>
        /// Returns all available metadata in the flac file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public static List<MetadataBlock> GetMetaData(string path)
        {
            using (FlacFile flac = new FlacFile(path))
            {
                return flac.Metadata;
            }
        }

        /// <summary>
        /// Returns the StreamInfo metadata.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>The StreamInfo metadata or null if no StreamInfo metadata is found.</returns>
        public static StreamInfo GetStreamInfo(string path)
        {
            using (FlacFile flac = new FlacFile(path))
            {
                return flac.StreamInfo;
            }
        }

        /// <summary>
        /// Returns the vorbis comment metadata (ID3V2 tags).
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>The vorbis comment metadata or null if none is available.</returns>
        public static VorbisComment GetVorbisComment(string path)
        {
            using (FlacFile flac = new FlacFile(path))
            {
                return flac.VorbisComment;
            }
        }

        #endregion

        #region Detail Fetchers

        /// <summary>
        /// Gets the specific vorbis field name (example = ARTIST) if it is available.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="fieldName"></param>
        /// <returns>The value of the field.</returns>
        public static VorbisCommentValues GetVorbisField(string path, string fieldName)
        {
            using (FlacFile flac = new FlacFile(path))
            {
                if (flac.VorbisComment != null && flac.VorbisComment.ContainsField(fieldName))
                {
                    return flac.VorbisComment[fieldName];
                }
                return new VorbisCommentValues();
            }
        }

        /// <summary>
        /// Gets the first artist of the track.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Empty string if the track number isn't specified in the metadata.</returns>
        public static string GetArtist(string path)
        {
            return GetVorbisField(path, "ARTIST").Value;
        }
        
        /// <summary>
        /// Gets the first title of the track.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Empty string if the track number isn't specified in the metadata.</returns>
        public static string GetTitle(string path)
        {
            return GetVorbisField(path, "TITLE").Value;
        }

        /// <summary>
        /// Gets the first album name.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Empty string if the track number isn't specified in the metadata.</returns>
        public static string GetAlbum(string path)
        {
            return GetVorbisField(path, "ALBUM").Value;
        }

        /// <summary>
        /// Gets the first track number.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Empty string if the track number isn't specified in the metadata.</returns>
        public static string GetTrackNumber(string path)
        {
            return GetVorbisField(path, "TRACKNUMBER").Value;
        }

        /// <summary>
        /// Gets the first genre of the track.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Empty string if no genre is specified in the metadata.</returns>
        public static string GetGenre(string path)
        {
            return GetVorbisField(path, "GENRE").Value;
        }

        /// <summary>
        /// Gets the duration of the track in seconds.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>The duration of the track in seconds or 0 if the duration is not known (sample count is missing from streaminfo metadata).</returns>
        public static int GetDuration(string path)
        {
            using (FlacFile file = new FlacFile(path))
            {
                return file.StreamInfo.Duration;
            }
        }

        #endregion


    }
}
