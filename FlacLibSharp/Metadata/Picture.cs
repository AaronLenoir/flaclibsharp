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
        private const uint FIXED_BLOCK_LENGTH = 8 * 4; // There are 8 32-bit fields = total block size - variable length data

        private PictureType pictureType;

        private string mimeType;

        private string description;

        private UInt32 width, height, colorDepth, colors;

        private byte[] data;

        private string url;

        public Picture()
        {
            this.Header.Type = MetadataBlockHeader.MetadataBlockType.Picture;
            this.mimeType = string.Empty;
            this.description = string.Empty;
            this.data = new byte[] {};
            CalculateMetadataBlockLength();
        }

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
            // This is where the header will come
            long headerPosition = targetStream.Position;
            // Moving along, we'll write the header last!
            targetStream.Seek(4, SeekOrigin.Current);

            // 32-bit picture type
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)this.pictureType), 0, 4);

            byte[] mimeTypeData = Encoding.ASCII.GetBytes(this.mimeType);
            // Length of the MIME type string (in bytes ...)
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)mimeTypeData.Length), 0, 4);
            // printable ascii characters (0x20 - 0x7e)
            for (int i = 0; i < mimeTypeData.Length; i++)
            {
                if (mimeTypeData[i] < 0x20 || mimeTypeData[i] > 0x7e)
                {
                    // Make sure we write the text correctly as specified by the format.
                    mimeTypeData[i] = 0x20;
                }
            }
            targetStream.Write(mimeTypeData, 0, mimeTypeData.Length);

            byte[] descriptionData = Encoding.UTF8.GetBytes(this.description);
            // Length of the description string (in bytes ...)
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)descriptionData.Length), 0, 4);
            // The description of the picture (in UTF-8)
            targetStream.Write(descriptionData, 0, descriptionData.Length);

            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)this.width), 0, 4);
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)this.height), 0, 4);
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)this.colorDepth), 0, 4);
            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)this.colors), 0, 4);

            targetStream.Write(BinaryDataHelper.GetBytesUInt32((uint)this.data.Length), 0, 4);
            targetStream.Write(this.data, 0, this.data.Length);

            // Writing the header, now we have the required information on the variable length fields
            CalculateMetadataBlockLength((uint)mimeTypeData.Length, (uint)descriptionData.Length, (uint)this.data.Length);
            
            long currentPosition = targetStream.Position;
            targetStream.Position = headerPosition;
            this.Header.WriteHeaderData(targetStream);
            targetStream.Position = currentPosition;
        }

        /// <summary>
        /// Calculates the total size of this block, taking into account the lengths of the variable length fields.
        /// </summary>
        private void CalculateMetadataBlockLength()
        {
            uint mimeLength = (uint)Encoding.ASCII.GetByteCount(this.mimeType);
            uint descriptionLength = (uint)Encoding.UTF8.GetByteCount(this.description);
            uint pictureDataLength = (uint)this.data.Length;

            CalculateMetadataBlockLength(mimeLength, descriptionLength, pictureDataLength);
        }

        /// <summary>
        /// Calculates the total size of this block, taking into account the lengths of the variable length fields.
        /// </summary>
        /// <param name="mimeLength"></param>
        /// <param name="descriptionLength"></param>
        /// <param name="pictureDataLength"></param>
        /// <remarks>If the lengths of the variable length fields are already available, use this function, otherwise use the parameterless override.</remarks>
        private void CalculateMetadataBlockLength(uint mimeLength, uint descriptionLength, uint pictureDataLength)
        {
            this.Header.MetaDataBlockLength = FIXED_BLOCK_LENGTH + mimeLength + descriptionLength + pictureDataLength;
        }

        /// <summary>
        /// What kind of picture this is.
        /// </summary>
        public PictureType PictureType { 
            get { return this.pictureType; }
            set { this.pictureType = value; }
        }

        /// <summary>
        /// The MIME type of the picture file.
        /// </summary>
        public string MIMEType {
            get { return this.mimeType; }
            set { this.mimeType = value; }
        }

        /// <summary>
        /// A description for the picture.
        /// </summary>
        public string Description {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Width of the picture (in pixels).
        /// </summary>
        public UInt32 Width {
            get { return this.width; }
            set { this.width = value; }
        }
        /// <summary>
        /// Height of the picture (in pixels).
        /// </summary>
        public UInt32 Height {
            get { return this.height; }
            set { this.height = value; }
        }
        /// <summary>
        /// The colour depth of the picture.
        /// </summary>
        public UInt32 ColorDepth {
            get { return this.colorDepth; }
            set { this.colorDepth = value; }
        }
        /// <summary>
        /// For color indexed pictures, all of the colours in the picture.
        /// </summary>
        public UInt32 Colors { get { return this.colors; } }

        /// <summary>
        /// The actual picture data in a stream
        /// </summary>
        public byte[] Data {
            get { return this.data; }
            set { this.data = value; }
        }

        /// <summary>
        /// The URL for the image if the MIME Type indicates a URL reference (MIME Type = '-->').
        /// </summary>
        public string URL { get { return this.url; } }

    }
}
