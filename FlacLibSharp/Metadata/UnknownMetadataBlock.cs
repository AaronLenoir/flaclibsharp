using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlacLibSharp {
    class FLACUnknownMetaDataBlock : MetadataBlock {

        public FLACUnknownMetaDataBlock()
        {
            this.Header.Type = MetadataBlockHeader.MetadataBlockType.None;
        }
        
        public override void LoadBlockData(byte[] data) {
            // We don't do anything, because this block format is unknown or unsupported...
        }

        /// <summary>
        /// When overridden in a derived class, will write the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            throw new NotImplementedException();
        }

    }
}
