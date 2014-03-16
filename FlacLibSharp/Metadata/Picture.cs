using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Exceptions;
using FlacLibSharp.Helpers;

namespace FlacLibSharp
{
    /// <summary>
    /// What kind of picture is in the flac file, picture type according to the ID3v2 APIC frame.
    /// </summary>
    public enum PictureType
    {
        /// <summary>
        /// A general picture.
        /// </summary>
        Other = 0,
        /// <summary>
        /// The picture is a file icon.
        /// </summary>
        FileIcon = 1,
        /// <summary>
        /// The picture is another file icon.
        /// </summary>
        OtherFileIcon = 2,
        /// <summary>
        /// The picture is the front cover of an album.
        /// </summary>
        CoverFront = 3,
        /// <summary>
        /// The picture is the back cover of an album.
        /// </summary>
        CoverBack = 4,
        /// <summary>
        /// The picture is the leaflet page of an album.
        /// </summary>
        LeafletPage = 5,
        /// <summary>
        /// The picture is a media page (e.g. label of CD).
        /// </summary>
        Media = 6,
        /// <summary>
        /// The picture of the lead artist.
        /// </summary>
        LeadArtist = 7,
        /// <summary>
        /// Picture of the artist.
        /// </summary>
        Artist = 8,
        /// <summary>
        /// Picture of the conductor.
        /// </summary>
        Conductor = 9,
        /// <summary>
        /// Picture of the band.
        /// </summary>
        Band = 10,
        /// <summary>
        /// picture of the composer.
        /// </summary>
        Composer = 11,
        /// <summary>
        /// Picture of the Lyricist.
        /// </summary>
        Lyricist = 12,
        /// <summary>
        /// Picture of the recording location.
        /// </summary>
        RecordingLocation = 13,
        /// <summary>
        /// Picture during the recording.
        /// </summary>
        DuringRecording = 14,
        /// <summary>
        /// Picture during the performance.
        /// </summary>
        DuringPerformance = 15,
        /// <summary>
        /// A movie screen capture picture.
        /// </summary>
        MovieScreenCapture = 16, 
        /// <summary>
        /// A picture of a bright coloured fish. Yes, really ... a fish. Brightly coloured even!
        /// </summary>
        BrightColouredFish = 17,
        /// <summary>
        /// A picture of an illustration.
        /// </summary>
        Illustration = 18,
        /// <summary>
        /// A picture of the artist logo.
        /// </summary>
        ArtistLogotype = 19,
        /// <summary>
        /// The studio logo.
        /// </summary>
        StudioLogotype = 20
    }

    /// <summary>
    /// A picture metadata block.
    /// </summary>
    public class Picture : MetadataBlock
    {
        private PictureType pictureType;

        private string mimeType;

        private string description;

        private UInt32 width, height, colorDepth, colors;

        private byte[] data;

        private string url;

        /// <summary>
        /// Loads the picture data from a Metadata block.
        /// </summary>
        /// <param name="data"></param>
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

        /// <summary>
        /// Will write the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// What kind of picture this is.
        /// </summary>
        public PictureType PictureType { get { return this.pictureType; } }

        /// <summary>
        /// The MIME type of the picture file.
        /// </summary>
        public string MIMEType { get { return this.mimeType; } }

        /// <summary>
        /// A description for the picture.
        /// </summary>
        public string Description { get { return this.description; } }

        /// <summary>
        /// Width of the picture (in pixels).
        /// </summary>
        public UInt32 Width { get { return this.width; } }
        /// <summary>
        /// Width of the picture (in pixels).
        /// </summary>
        public UInt32 Height { get { return this.height; } }
        /// <summary>
        /// The colour depth of the picture.
        /// </summary>
        public UInt32 ColorDepth { get { return this.colorDepth; } }
        /// <summary>
        /// For color indexed pictures, all of the colours in the picture.
        /// </summary>
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
