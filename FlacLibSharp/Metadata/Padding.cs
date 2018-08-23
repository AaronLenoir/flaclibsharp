using System;
using System.IO;
using FlacLibSharp.Exceptions;

namespace FlacLibSharp {
    /// <summary>
    /// Empty space inside the flac file ... a number of meaningless bits.
    /// </summary>
    public class Padding : MetadataBlock {

        private UInt32 emptyBitCount;

        /// <summary>
        /// Creates an empty metadata block.
        /// </summary>
        public Padding()
        {
            this.Header.Type = MetadataBlockHeader.MetadataBlockType.Padding;
            this.emptyBitCount = 0;
        }

        /// <summary>
        /// Loads the padding data from the given data.
        /// </summary>
        public override void LoadBlockData(byte[] data) {
            this.emptyBitCount = (UInt32)(this.Header.MetaDataBlockLength * 8);
        }

        /// <summary>
        /// Writes the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            this.Header.WriteHeaderData(targetStream);
            
            // write a bunch of 0 bytes (probably shouldn't do this byte per byte ...)
            UInt32 bytes = this.emptyBitCount / 8;
            for (UInt32 i = 0; i < bytes; i++)
            {
                targetStream.WriteByte(0);
            }
        }

        /// <summary>
        /// How many empty bits there are in the padding, must be a multiple of eight.
        /// </summary>
        public UInt32 EmptyBitCount {
            get
            {
                return this.emptyBitCount;
            }
            set
            {
                if (value % 8 != 0)
                {
                    throw new FlacLibSharpInvalidPaddingBitCount(String.Format("Padding for {0} bits is impossible, the bitcount must be a multiple of eight.", value));
                }
            
                this.emptyBitCount = value;
                this.Header.MetaDataBlockLength = value / 8;
            }
        }

    }
}
