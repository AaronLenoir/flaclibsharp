using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Metadata {
    class FLACUnknownMetaDataBlock : MetadataBlock {
        public override void LoadBlockData(byte[] data) {
            // We don't do anything, because this block format is unknown or unsupported...
        }
    }
}
