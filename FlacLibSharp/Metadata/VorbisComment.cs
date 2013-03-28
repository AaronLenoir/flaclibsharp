﻿using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp.Metadata
{
    public class VorbisComment : MetadataBlock
    {
        // Vorbis format: http://www.xiph.org/vorbis/doc/v-comment.html

        private Dictionary<string, string> comments;
        private string vendor;

        public override void LoadBlockData(byte[] data)
        {
            if (this.comments == null)
            {
                this.comments = new Dictionary<string, string>();
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

        protected void AddComment(string comment) {
            

            int splitIndex = comment.IndexOf("=");
            string key = comment.Substring(0, splitIndex).ToUpper();
            string value = comment.Substring(splitIndex + 1);

            this.comments.Add(key, value);
        }

        public string Vendor { get { return this.vendor; } }

        public String this[string key]
        {
            get
            {
                return this.comments[key.ToUpper()];
            }
        }

        public bool ContainsField(string key)
        {
            return this.comments.ContainsKey(key.ToUpper());
        }

        /// <summary>
        /// Gets the Artist if available, if not an empty string is returned.
        /// </summary>
        public string Artist { get { if (this.ContainsField("ARTIST")) return this["ARTIST"]; else return string.Empty; } }

        /// <summary>
        /// Gets the Title if available, if not an empty string is returned.
        /// </summary>
        public string Title { get { if (this.ContainsField("TITLE")) return this["TITLE"]; else return string.Empty; } }

        /// <summary>
        /// Gets the Album if available, if not an empty string is returned.
        /// </summary>
        public string Album { get { if (this.ContainsField("ALBUM")) return this["ALBUM"]; else return string.Empty; } }

        /// <summary>
        /// Gets the Date if available, if not an empty string is returned.
        /// </summary>
        public string Date { get { if (this.ContainsField("DATE")) return this["DATE"]; else return string.Empty; } }

        /// <summary>
        /// Gets the TrackNumber if available, if not an empty string is returned.
        /// </summary>
        public string TrackNumber { get { if (this.ContainsField("TRACKNUMBER")) return this["TRACKNUMBER"]; else return string.Empty; } }

        /// <summary>
        /// Gets the Genre if available, if not an empty string is returned.
        /// </summary>
        public string Genre { get { if (this.ContainsField("GENRE")) return this["GENRE"]; else return string.Empty; } }

    }
}
