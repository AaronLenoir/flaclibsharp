using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp {
    /// <summary>
    /// Empty space inside the flac file ... a number of meaningless bits.
    /// </summary>
    public class Padding : MetadataBlock {

        private UInt32 emptyBitCount;

        /// <summary>
        /// Loads the padding data from the given data.
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data) {
            this.emptyBitCount = (UInt32)(this.Header.MetaDataBlockLength * 8);
        }

        /// <summary>
        /// How many empty bits there are in the padding.
        /// </summary>
        public UInt32 EmptyBitCount {
            get { return this.emptyBitCount; }
            set { this.emptyBitCount = value; }
        }

    }
}
