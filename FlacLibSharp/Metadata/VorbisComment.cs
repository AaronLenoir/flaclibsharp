using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlacLibSharp.Exceptions;
using FlacLibSharp.Helpers;

namespace FlacLibSharp
{
    /// <summary>
    /// The value or values of a single Vorbis Comment Field.
    /// </summary>
    public class VorbisCommentValues : List<string>
    {
        /// <summary>
        /// Creates an empty vorbis comment.
        /// </summary>
        public VorbisCommentValues() { }

        /// <summary>
        /// Creates a vorbis comment with one value.
        /// </summary>
        public VorbisCommentValues(string value)
        {
            this.Add(value);
        }

        /// <summary>
        /// Creates a vorbis comment with the given values.
        /// </summary>
        public VorbisCommentValues(IEnumerable<string> values)
        {
            this.AddRange(values);
        }

        /// <summary>
        /// The first value of the list of values.
        /// </summary>
        /// <remarks></remarks>
        public string Value {
            get
            {
                if (this.Count == 0) { return string.Empty; }
                return this[0];
            }
            set
            {
                if (this.Count == 0) { this.Add(value); }
                else { this[0] = value; }
            }
        }

        /// <summary>
        /// If the comment has at least one value, the first value is returned, otherwise an empty string.
        /// </summary>
        public override string ToString()
        {
            return this.Value;
        }
    }

    /// <summary>
    /// A metadata block contain the "Vorbis Comment" (artist, ...)
    /// </summary>
    public class VorbisComment : MetadataBlock, IEnumerable<KeyValuePair<string, VorbisCommentValues>>
    {
        // Vorbis format: http://www.xiph.org/vorbis/doc/v-comment.html

        private Dictionary<string, VorbisCommentValues> comments;
        private string vendor;

        /// <summary>
        /// Initializes a Vorbis comment block (without any content).
        /// </summary>
        public VorbisComment()
        {
            this.Header.Type = MetadataBlockHeader.MetadataBlockType.VorbisComment;
            this.comments = new Dictionary<string, VorbisCommentValues>(StringComparer.OrdinalIgnoreCase);
            this.vendor = string.Empty;
        }

        /// <summary>
        /// Loads the Vorbis from a block of data.
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data)
        {
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

            // In FlacLibSharp a single comment can have multiple values, but
            // in the FLAC format each value is a comment by itself. So
            // we can't use this.comments.Count, since 1 comment could have 10
            // values, which results in 10 comments in the FLAC file.
            var totalValueCount = 0;
            foreach(var comment in this.comments)
            {
                foreach (var value in comment.Value)
                {
                    totalValueCount++;
                }
            }
            number = BinaryDataHelper.GetBytesUInt32((uint)totalValueCount);
            targetStream.Write(BinaryDataHelper.SwitchEndianness(number, 0, 4), 0, 4);
            totalLength += 4;

            foreach (var comment in this.comments)
            {
                foreach (var value in comment.Value)
                {
                    string commentText = string.Format("{0}={1}", comment.Key, value);
                    byte[] commentData = System.Text.Encoding.UTF8.GetBytes(commentText);
                    number = BinaryDataHelper.GetBytesUInt32((uint)commentData.Length);
                    targetStream.Write(BinaryDataHelper.SwitchEndianness(number, 0, 4), 0, 4);
                    targetStream.Write(commentData, 0, commentData.Length);
                    totalLength += 4 + (uint)commentData.Length;
                }
            }

            long endPosition = targetStream.Position;

            targetStream.Seek(headerPosition, SeekOrigin.Begin);

            this.Header.MetaDataBlockLength = totalLength;
            this.Header.WriteHeaderData(targetStream);

            targetStream.Seek(endPosition, SeekOrigin.Begin);

            // Note: FLAC does NOT have the framing bit for vorbis so we don't have to write this.
        }

        /// <summary>
        /// Returns an enumerator that iterates through all VorbisComments.
        /// </summary>
        public IEnumerator<KeyValuePair<string, VorbisCommentValues>> GetEnumerator()
        {
            return this.comments.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all VorbisComments.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.comments.GetEnumerator();
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

        protected void AddComment(string fieldName, VorbisCommentValues values)
        {
            if (this.comments.ContainsKey(fieldName))
            {
                this.comments[fieldName].AddRange(values);
            }
            else
            {
                this.comments.Add(fieldName, values);
            }
        }

        /// <summary>
        /// Adds a comment to the list of vorbis comments.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        protected void AddComment(string fieldName, string value)
        {
            if (this.comments.ContainsKey(fieldName))
            {
                this.comments[fieldName].Add(value);
            } else
            {
                this.comments.Add(fieldName, new VorbisCommentValues(value));
            }
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
        public VorbisCommentValues this[string key]
        {
            get
            {
                if (!this.comments.ContainsKey(key))
                {
                    this.comments.Add(key, new VorbisCommentValues());
                }

                return this.comments[key];
            }
            set
            {
                if (!this.comments.ContainsKey(key))
                {
                    this.comments.Add(key, value);
                } else
                {
                    this.comments[key] = value;
                }
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
        /// Removes all Vobis Comment values for the given key.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be removed.</param>
        /// <remarks>Does nothing if no Vorbis Comments with the key are found.</remarks>
        public void Remove(string key)
        {
            if (this.comments.ContainsKey(key))
            {
                this.comments.Remove(key);
            }
        }

        /// <summary>
        /// Removes the given value from the VorbisComment.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be removed.</param>
        /// <remarks>Does nothing if no Vorbis Comments with the key are found.</remarks>
        public void Remove(string key, string value)
        {
            if (this.comments.ContainsKey(key))
            {
                for(var i = this.comments[key].Count - 1; i >= 0; i--)
                {
                    if (this.comments[key][i].Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        this.comments[key].RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new Vorbis Comment with the given key and a single value.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be added.</param>
        /// <param name="value">The value for this comment.</param>
        /// <remarks>If a tag with the key already exists, the value is appended.</remarks>
        public void Add(string key, string value)
        {
            if (this.ContainsField(key))
            {
                this[key].Add(value);
            } else
            {
                this[key] = new VorbisCommentValues(value);
            }
        }

        /// <summary>
        /// Adds a new Vorbis Comment with the given key and a list of values.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be removed.</param>
        /// <param name="values">The values for this comment.</param>
        /// <remarks>If a tag with the key already exists, the values are appended.</remarks>
        public void Add(string key, IEnumerable<string> values)
        {
            if (this.ContainsField(key))
            {
                this[key].AddRange(values);
            } else
            {
                this[key] = new VorbisCommentValues(values);
            }
        }

        /// <summary>
        /// Replaces all the values (if any) for a given tag with the given value.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be replaced.</param>
        /// <param name="values">The values for this comment.</param>
        public void Replace(string key, string value)
        {
            this[key] = new VorbisCommentValues(value);
        }

        /// <summary>
        /// Replaces all the values (if any) for a given tag with the given value.
        /// </summary>
        /// <param name="key">The key of the vorbis comment field to be replaced.</param>
        /// <param name="values">The values for this comment.</param>
        public void Replace(string key, IEnumerable<string> values)
        {
            this[key] = new VorbisCommentValues(values);
        }

        /// <summary>
        /// Gets or sets the Artist if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public VorbisCommentValues Artist {
            get { return this["ARTIST"]; }
            set { this["ARTIST"] = value; }
        }

        /// <summary>
        /// Gets or sets the Title if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public VorbisCommentValues Title {
            get { return this["TITLE"]; }
            set { this["TITLE"] = value; }
        }

        /// <summary>
        /// Gets or sets the Album if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public VorbisCommentValues Album {
            get { return this["ALBUM"]; }
            set { this["ALBUM"] = value; }
        }

        /// <summary>
        /// Gets or sets the Date if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public VorbisCommentValues Date {
            get { return this["DATE"]; }
            set { this["DATE"] = value; }
        }

        /// <summary>
        /// Gets or sets the Tacknumber if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public VorbisCommentValues TrackNumber {
            get { return this["TRACKNUMBER"]; }
            set { this["TRACKNUMBER"] = value; }
        }

        /// <summary>
        /// Gets or sets the Genre if available.
        /// </summary>
        /// <remarks>If not found an empty string is returned.</remarks>
        public VorbisCommentValues Genre {
            get { return this["GENRE"]; }
            set { this["GENRE"] = value; }
        }

        /// <summary>
        /// Gets or sets the embedded CueSheet if available.
        /// </summary>
        /// <remarks>If not found an empty VorbisCommentValues instance is returned.</remarks>
        public VorbisCommentValues CueSheet
        {
            get { return this["CUESHEET"]; }
            set { this["CUESHEET"] = value; }
        }
    }
}
