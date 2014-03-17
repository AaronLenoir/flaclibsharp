using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A metadata block header.
    /// </summary>
    public class MetadataBlockHeader {

        /// <summary>
        /// Defines the type of meta data.
        /// </summary>
        public enum MetadataBlockType {
            /// <summary>
            /// An unknown type of metadata.
            /// </summary>
            None,
            /// <summary>
            /// Information on the flac audio stream.
            /// </summary>
            StreamInfo,
            /// <summary>
            /// A metadata block that pads some space. It has no further meaning.
            /// </summary>
            Padding,
            /// <summary>
            /// A metadata block with application specific information.
            /// </summary>
            Application,
            /// <summary>
            /// A metadata block that has some information for seektables.
            /// </summary>
            Seektable,
            /// <summary>
            /// A metadata block that contains the vorbis comments (artist, field, ...)
            /// </summary>
            VorbisComment,
            /// <summary>
            /// A metadata block containing cue sheet information.
            /// </summary>
            CueSheet,
            /// <summary>
            /// A metadata block with picture information.
            /// </summary>
            Picture,
            /// <summary>
            /// A metadata block that is not valid or could not be parsed.
            /// </summary>
            Invalid
        }

        /// <summary>
        /// Creates a new metadata block header from the provided data.
        /// </summary>
        /// <param name="data"></param>
        public MetadataBlockHeader(byte[] data) {
            this.ParseData(data);
        }

        /// <summary>
        /// Will write the data representing this header (as it is stored in the FLAC file) to the given stream.
        /// </summary>
        /// <param name="targetStream">The stream where the data will be written to.</param>
        public void WriteHeaderData(Stream targetStream) {
            byte data = this.isLastMetaDataBlock ? (byte)128 : (byte)0; // The 128 because the last metadata flag is the most significant bit set to 1 ...
            data += (byte)(this.typeID & 0x7F); // We make sure to chop off the last bit

            targetStream.Write(new byte[] { data }, 0, 1);

            // 24-bit metaDataBlockLength
            targetStream.Write(BinaryDataHelper.GetBytes((UInt64)this.metaDataBlockLength, 3), 0, 3);
        }

        private bool isLastMetaDataBlock;

        private int typeID;

        /// <summary>
        /// Indicates if this is the last metadata block in the file (meaning that it is followed by the actual audio stream).
        /// </summary>
        public bool IsLastMetaDataBlock {
            get { return this.isLastMetaDataBlock; }
            set { this.isLastMetaDataBlock = value; }
        }

        private MetadataBlockType type;

        /// <summary>
        /// Defines what kind of metadatablock this is.
        /// </summary>
        public MetadataBlockType Type {
            get { return this.type; }
            set {
                this.type = value;
                typeID = (int)value;
            }
        }

        private UInt32 metaDataBlockLength;

        /// <summary>
        /// Defines the length of the metadata block.
        /// </summary>
        public UInt32 MetaDataBlockLength {
            get { return this.metaDataBlockLength; }
            set { this.metaDataBlockLength = value; }
        }

        /// <summary>
        /// Interprets the meta data block header.
        /// </summary>
        /// <param name="data"></param>
        protected void ParseData(byte[] data) {
            // Parses the 4 byte header data:
            // Bit 1:   Last-metadata-block flag: '1' if this block is the last metadata block before the audio blocks, '0' otherwise.
            // Bit 2-8: Block Type, 
            //  0 : STREAMINFO 
            //  1 : PADDING 
            //  2 : APPLICATION 
            //  3 : SEEKTABLE 
            //  4 : VORBIS_COMMENT 
            //  5 : CUESHEET 
            //  6 : PICTURE 
            //  7-126 : reserved 
            //  127 : invalid, to avoid confusion with a frame sync code
            // Next 3 bytes: Length (in bytes) of metadata to follow (does not include the size of the METADATA_BLOCK_HEADER)

            this.isLastMetaDataBlock = BinaryDataHelper.GetBoolean(data, 0, 0);

            typeID = data[0] & 0x7F;
            switch (typeID) {
                case 0:
                    this.type = MetadataBlockType.StreamInfo;
                    this.metaDataBlockLength = 34;
                    break;
                case 1:
                    this.type = MetadataBlockType.Padding;
                    break;
                case 2:
                    this.type = MetadataBlockType.Application;
                    break;
                case 3:
                    this.type = MetadataBlockType.Seektable;
                    break;
                case 4:
                    this.type = MetadataBlockType.VorbisComment;
                    break;
                case 5:
                    this.type = MetadataBlockType.CueSheet;
                    break;
                case 6:
                    this.type = MetadataBlockType.Picture;
                    break;
            }
            if (typeID > 6 && typeID < 127) {
                this.type = MetadataBlockType.None;
            } else if(typeID >= 127) {
                this.type = MetadataBlockType.Invalid;
            }

            this.metaDataBlockLength = (BinaryDataHelper.GetUInt24(data, 1));
        }
    }
}
