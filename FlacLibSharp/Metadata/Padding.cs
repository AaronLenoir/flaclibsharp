using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Metadata {
    public class Padding : MetadataBlock {

        private UInt32 emptyBitCount;

        public override void LoadBlockData(byte[] data) {
            this.emptyBitCount = (UInt32)(this.Header.MetaDataBlockLength * 8);
        }

        public UInt32 EmptyBitCount {
            get { return this.emptyBitCount; }
            set { this.emptyBitCount = value; }
        }

    }
}
