using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Exceptions;
using FlacLibSharp.Helpers;

namespace FlacLibSharp
{
    public enum PictureType
    {
        Other = 0, FileIcon = 1, OtherFileIcon = 2, CoverFront = 3, CoverBack = 4,
        LeafletPage = 5, Media = 6, LeadArtist = 7, Artist = 8, Conductor = 9,
        Band = 10, Composer = 11, Lyricist = 12, RecordingLocation = 13, DuringRecording = 14,
        DuringPerformance = 15, MovieScreenCapture = 16, BrightColouredFish = 17,
        Illustration = 18, ArtistLogotype = 19, StudioLogotype = 20
    }

    public class Picture : MetadataBlock
    {
        private PictureType pictureType;

        private string mimeType;

        private string description;

        private UInt32 width, height, colorDepth, colors;

        private byte[] data;

        private string url;

        public override void LoadBlockData(byte[] data)
        {
            // First 32-bit: picture type according to the ID3v2 APIC frame
            pictureType = (PictureType)(int)BinaryDataHelper.GetUInt32(data, 0);

            // Then the length of the MIME type text (32-bit) and the mime type
            int mimeTypeLength = (int)BinaryDataHelper.GetUInt32(data, 4);
            byte[] mimeData = BinaryDataHelper.GetDataSubset(data, 8, mimeTypeLength);
            this.mimeType = Encoding.ASCII.GetString(mimeData);

            int byteOffset = 8 + mimeTypeLength;

            // Then the description (in UTF-8)
            int descriptionLength = (int)BinaryDataHelper.GetUInt32(data, byteOffset);
            byte[] descriptionData = BinaryDataHelper.GetDataSubset(data, byteOffset + 4, descriptionLength);
            this.description = Encoding.UTF8.GetString(descriptionData);

            byteOffset += 4 + descriptionLength;

            this.width = BinaryDataHelper.GetUInt32(data, byteOffset);
            this.height = BinaryDataHelper.GetUInt32(data, byteOffset + 4);
            this.colorDepth = BinaryDataHelper.GetUInt32(data, byteOffset + 8);
            this.colors = BinaryDataHelper.GetUInt32(data, byteOffset + 12);

            byteOffset += 16;

            int dataLength = (int)BinaryDataHelper.GetUInt32(data, byteOffset);
            this.data = BinaryDataHelper.GetDataSubset(data, byteOffset + 4, dataLength);
            if (mimeType == "-->")
            {
                this.url = Encoding.UTF8.GetString(this.data);
            }
        }

        public PictureType PictureType { get { return this.pictureType; } }

        public string MIMEType { get { return this.mimeType; } }

        public string Description { get { return this.description; } }

        public UInt32 Width { get { return this.width; } }
        public UInt32 Height { get { return this.height; } }
        public UInt32 ColorDepth { get { return this.colorDepth; } }
        public UInt32 Colors { get { return this.colors; } }

        /// <summary>
        /// The actual picture data in a stream
        /// </summary>
        public byte[] Data { get { return this.data; } }

        /// <summary>
        /// The URL for the image if the MIME Type indicates a URL reference (MIME Type = '-->').
        /// </summary>
        public string URL { get { return this.url; } }

    }
}
