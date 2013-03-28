using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FlacLibSharp.Metadata {
    public abstract class MetadataBlock {

        protected MetadataBlock()
        { }

        public abstract void LoadBlockData(byte[] data);

        /// <summary>
        /// Loads the first meta data block found on the stream (first byte is supposed to be the start of the meta data block)... the stream will be forwarded to the start
        /// of the next meta data block (or the start of the audio frames)
        /// </summary>
        /// <param name="data"></param>
        public static MetadataBlock Create(Stream data)
        {
            // Step 1: Get header
            byte[] headerData = new byte[4]; // always 4 bytes
            
            data.Read(headerData, 0, 4);
            MetadataBlockHeader header = new MetadataBlockHeader(headerData);

            // Step 2: Get instance of meta data block by type

            MetadataBlock metaDataBlock = GetInstanceByBlockType(header.Type);

            if (metaDataBlock != null) {
                metaDataBlock.header = header;

                // Step 3: Read block of meta data (according to header)

                byte[] blockData = new byte[header.MetaDataBlockLength];
                data.Read(blockData, 0, header.MetaDataBlockLength);

                // Step 4: Load the meta data into the meta data block instance

                metaDataBlock.LoadBlockData(blockData);
            }

            // Step 5: Return Meta Data Block...
            return metaDataBlock;
        }

        private static MetadataBlock GetInstanceByBlockType(MetadataBlockHeader.MetadataBlockType type)
        {
            // TODO: The picture block (http://flac.sourceforge.net/format.html#metadata_block_picture)
            switch (type) {
                case MetadataBlockHeader.MetadataBlockType.StreamInfo:
                    return new StreamInfo();
                case MetadataBlockHeader.MetadataBlockType.Application:
                    return new ApplicationInfo();
                case MetadataBlockHeader.MetadataBlockType.Padding:
                    return new Padding();
                case MetadataBlockHeader.MetadataBlockType.Seektable:
                    return new SeekTable();
                case MetadataBlockHeader.MetadataBlockType.CueSheet:
                    return new CueSheet();
                case MetadataBlockHeader.MetadataBlockType.VorbisComment:
                    return new VorbisComment();
                case MetadataBlockHeader.MetadataBlockType.Picture:
                    return new Picture();
                default:
                    return new FLACUnknownMetaDataBlock(); // Unknown/Unsupported/Unimplemented datablock type
            }
        }

        private MetadataBlockHeader header;

        public MetadataBlockHeader Header {
            get { return this.header; }
        }

    }
}
