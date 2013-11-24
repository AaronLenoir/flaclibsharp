using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlacLibSharp
{
    /// <summary>
    /// Parses FLAC data from the given stream of file.
    /// </summary>
    /// <remarks>Only metadata parsing is currently implemented, decoding frames is THE big TODO.</remarks>
    public class FlacFile : IDisposable
    {
        private Stream dataStream;
        private List<MetadataBlock> metadata;

        private readonly byte[] magicFlacMarker = { 0x66, 0x4C, 0x61, 0x43 }; // fLaC

        #region Constructors

        /// <summary>
        /// Open a Flac File
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public FlacFile(string path)
        {
            this.dataStream = File.OpenRead(path);
            this.Initialize();
        }

        /// <summary>
        /// Open a flac file from a stream of data
        /// </summary>
        /// <param name="data">Any stream of data</param>
        /// <remarks>Stream is assumed to be at the beginning of the FLAC data</remarks>
        public FlacFile(Stream data)
        {
            this.dataStream = data;
            this.Initialize();
        }

        #endregion

        #region Properties

        public List<MetadataBlock> Metadata
        {
            get
            {
                if (this.metadata == null)
                {
                    this.metadata = new List<MetadataBlock>();
                }
                return this.metadata;
            }
        }

        #endregion

        #region Initialization

        public void Initialize()
        {
            VerifyFlacIdentity();
            ReadMetadata();
        }

        public void VerifyFlacIdentity()
        {
            byte[] data = new byte[4];

            try
            {
                this.dataStream.Read(data, 0, 4);
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != magicFlacMarker[i])
                    {
                        throw new FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException("In Verify Flac Identity");
                    }
                }
            }
            catch (ArgumentException)
            {
                throw new FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException("In Verify Flac Identity");
            }
        }

        public void ReadMetadata()
        {
            bool foundStreamInfo = false;
            MetadataBlock lastMetaDataBlock = null;
            do
            {
                lastMetaDataBlock = MetadataBlock.Create(this.dataStream);
                this.Metadata.Add(lastMetaDataBlock);
                switch(lastMetaDataBlock.Header.Type) {
                    case MetadataBlockHeader.MetadataBlockType.StreamInfo:
                        foundStreamInfo = true;
                        this.streamInfo = (StreamInfo)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.Application:
                        this.applicationInfo = (ApplicationInfo)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.CueSheet:
                        this.cueSheet = (CueSheet)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.Picture:
                        this.picture = (Picture)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.Seektable:
                        this.seekTable = (SeekTable)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.VorbisComment:
                        this.vorbisComment = (VorbisComment)lastMetaDataBlock;
                        break;
                }
            } while (!lastMetaDataBlock.Header.IsLastMetaDataBlock);
            
            if (!foundStreamInfo)
                throw new Exceptions.FlacLibSharpStreamInfoMissing();
        }

        /* Direct access to different meta data */

        private StreamInfo streamInfo;
        private Picture picture;
        private ApplicationInfo applicationInfo;
        private VorbisComment vorbisComment;
        private CueSheet cueSheet;
        private SeekTable seekTable;

        public StreamInfo StreamInfo { get { return this.streamInfo; } }
        public Picture Picture { get { return this.picture; } }
        public ApplicationInfo ApplicationInfo { get { return this.applicationInfo; } }
        public VorbisComment VorbisComment { get { return this.vorbisComment; } }
        public CueSheet CueSheet { get { return this.cueSheet; } }
        public SeekTable SeekTable { get { return this.seekTable; } }

        #endregion

        public void Dispose()
        {
            if (dataStream != null)
            {
                dataStream.Close();
                dataStream = null;
            }
        }
    }
}
