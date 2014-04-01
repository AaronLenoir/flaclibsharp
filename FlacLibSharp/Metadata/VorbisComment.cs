using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp
{
    /// <summary>
    /// A metadata block contain the "Vorbis Comment" (artist, ...)
    /// </summary>
    public class VorbisComment : MetadataBlock
    {
        // Vorbis format: http://www.xiph.org/vorbis/doc/v-comment.html

        private Dictionary<string, string> comments;
        private string vendor;

        /// <summary>
        /// Loads the Vorbis from a block of data.
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data)
        {
            if (this.comments == null)
            {
                this.comments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            UInt32 vendorLength = BinaryDataHelper.GetUInt32(BinaryDataHelper.SwitchEndianness(data, 0, 4), 0);
            this.vendor = Encoding.UTF8.GetString(BinaryDataHelper.GetDataSubset(data, 4, (int)vendorLength));

            int startOfComments = 4 + (int)vendorLength;
            UInt32 userCommentListLength = BinaryDataHelper.GetUInt32(BinaryDataHelper.SwitchEndianness(data, startOfComments, 4), 0);
            // Start of comments actually four bytes further (first piece is the count of items in the list)
            startOfComments += 4;
            for (UInt32 i = 0; i < userCommentListLength; i++)
            {
                UInt32 commentLength = BinaryDataHelper.GetUInt32(BinaryDataHelper.SwitchEndianness(data, startOfComments, 4), 0);
                string comment = Encoding.UTF8.GetString(BinaryDataHelper.GetDataSubset(data, startOfComments + 4, (int)commentLength));
                // We're moving on in the array ...
                startOfComments += 4 + (int)commentLength;

                AddComment(comment);
            }

            // All done, note that FLAC doesn't have the "fraiming bit" for vorbis ...
        }

        /// <summary>
        /// Will write the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            uint totalLength = 0;

            long headerPosition = targetStream.Position;

            this.Header.WriteHeaderData(targetStream);

            // Write the vendor string (first write the length as a 32-bit uint and then the actual bytes
            byte[] vendorData = System.Text.Encoding.UTF8.GetBytes(this.vendor);
            byte[] number = BinaryDataHelper.GetBytesUInt32((uint)vendorData.Length);
            targetStream.Write(BinaryDataHelper.SwitchEndianness(number, 0, 4), 0, 4);
            targetStream.Write(vendorData, 0, vendorData.Length);
            totalLength += 4 + (uint)vendorData.Length;

            // Length of list of user comments (first a 32-bit uint, then the actual comments)
            number = BinaryDataHelper.GetBytesUInt32((uint)this.comments.Count);
            targetStream.Write(BinaryDataHelper.SwitchEndianness(number, 0, 4), 0, 4);
            totalLength += 4;

            foreach (var comment in this.comments)
            {
                string commentText = string.Format("{0}={1}", comment.Key, comment.Value);
                byte[] commentData = System.Text.Encoding.UTF8.GetBytes(commentText);
                number = BinaryDataHelper.GetBytesUInt32((uint)commentData.Length);
                targetStream.Write(BinaryDataHelper.SwitchEndianness(number, 0, 4), 0, 4);
                targetStream.Write(commentData, 0, commentData.Length);
                totalLength += 4 + (uint)commentData.Length;
            }

            long endPosition = targetStream.Position;

            targetStream.Seek(headerPosition, SeekOrigin.Begin);

            this.Header.MetaDataBlockLength = totalLength;
            this.Header.WriteHeaderData(targetStream);

            targetStream.Seek(endPosition, SeekOrigin.Begin);

            // Note: FLAC does NOT have the framing bit for vorbis so we don't have to write this.
        }

        /// <summary>
        /// Adds a comment to the list of vorbis comments.
        /// </summary>
        /// <param name="comment"></param>
        protected void AddComment(string comment)
        {
            int splitIndex = comment.IndexOf('=');
            string key = comment.Substring(0, splitIndex);
            string value = comment.Substring(splitIndex + 1);

            AddComment(key, value);
        }

        /// <summary>
        /// Adds a comment to the list of vorbis comments.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        protected void AddComment(string fieldName, string value)
        {
            this.comments.Add(fieldName, value);
        }

        /// <summary>
        /// The Vendor of the flac file.
        /// </summary>
        public string Vendor { get { return this.vendor; } }

        /// <summary>
        /// Get one of the vorbis comment.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field.</param>
        /// <returns>The value of the vorbis comment field.</returns>
        public String this[string key]
        {
            get
            {
                return this.comments[key];
            }
            set
            {
                this.comments[key] = value;
            }
        }

        /// <summary>
        /// Checks whether a field with the given key is present in the Vorbis Comment data.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field.</param>
        /// <returns>True if such a field is available.</returns>
        public bool ContainsField(string key)
        {
            return this.comments.ContainsKey(key);
        }

        /// <summary>
        /// Gets or sets the Artist if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public string Artist {
            get { if (this.ContainsField("ARTIST")) return this["ARTIST"]; else return string.Empty; }
            set { if (this.ContainsField("ARTIST")) this["ARTIST"] = value; else AddComment("ARTIST", value); }
        }

        /// <summary>
        /// Gets or sets the Title if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public string Title {
            get { if (this.ContainsField("TITLE")) return this["TITLE"]; else return string.Empty; }
            set { if (this.ContainsField("TITLE")) this["TITLE"] = value; else AddComment("TITLE", value); }
        }

        /// <summary>
        /// Gets or sets the Album if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public string Album {
            get { if (this.ContainsField("ALBUM")) return this["ALBUM"]; else return string.Empty; }
            set { if (this.ContainsField("ALBUM")) this["ALBUM"] = value; else AddComment("ALBUM", value); }
        }

        /// <summary>
        /// Gets or sets the Date if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public string Date {
            get { if (this.ContainsField("DATE")) return this["DATE"]; else return string.Empty; }
            set { if (this.ContainsField("DATE")) this["DATE"] = value; else AddComment("DATE", value); }
        }

        /// <summary>
        /// Gets or sets the Tacknumber if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public string TrackNumber {
            get { if (this.ContainsField("TRACKNUMBER")) return this["TRACKNUMBER"]; else return string.Empty; }
            set { if (this.ContainsField("TRACKNUMBER")) this["TRACKNUMBER"] = value; else AddComment("TRACKNUMBER", value); }
        }

        /// <summary>
        /// Gets or sets the Genre if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public string Genre {
            get { if (this.ContainsField("GENRE")) return this["GENRE"]; else return string.Empty; }
            set { if (this.ContainsField("GENRE")) this["GENRE"] = value; else AddComment("GENRE", value); }
        }

    }
}
