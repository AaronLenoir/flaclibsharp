using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    public class MetadataBlockHeader {

        public enum MetadataBlockType {
            None, StreamInfo, Padding, Application, Seektable, VorbisComment, CueSheet, Picture, Invalid
        }

        public MetadataBlockHeader(byte[] data) {
            this.ParseData(data);
        }

        private bool isLastMetaDataBlock;

        public bool IsLastMetaDataBlock {
            get { return this.isLastMetaDataBlock; }
            set { this.isLastMetaDataBlock = value; }
        }

        private MetadataBlockType type;

        public MetadataBlockType Type {
            get { return this.type; }
            set { this.type = value; }
        }

        private Int32 metaDataBlockLength;

        public Int32 MetaDataBlockLength {
            get { return this.metaDataBlockLength; }
            set { this.metaDataBlockLength = value; }
        }

        protected void ParseData(byte[] data) {
            int typeID;
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

            this.isLastMetaDataBlock = BinaryDataHelper.GetUInt(data, 0, 1, 0) == 1;

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

            this.metaDataBlockLength = (int)(BinaryDataHelper.GetUInt24(data, 1));
        }
    }
}
