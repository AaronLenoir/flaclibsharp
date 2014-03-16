using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlacLibSharp.Exceptions;

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
        
        // Some housekeeping for the file save
        private long frameStart;
        private string filePath = String.Empty;

        private static readonly byte[] magicFlacMarker = { 0x66, 0x4C, 0x61, 0x43 }; // fLaC

        #region Constructors

        /// <summary>
        /// Open a Flac File
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public FlacFile(string path)
        {
            this.filePath = path;
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

        /// <summary>
        /// A list of all the available metadata.
        /// </summary>
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

        /// <summary>
        /// Verifies the flac identity and tries to load the available metadata blocks.
        /// </summary>
        protected void Initialize()
        {
            VerifyFlacIdentity();
            ReadMetadata();
        }

        /// <summary>
        /// Verifies whether or not the first four bytes of the file indicate this is a flac file.
        /// </summary>
        private void VerifyFlacIdentity()
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

        /// <summary>
        /// Tries to parse all the metadata blocks available in the file.
        /// </summary>
        protected void ReadMetadata()
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
            
            // Remember where the frame data starts
            frameStart = this.dataStream.Position;
        }

        /* Direct access to different meta data */

        private StreamInfo streamInfo;
        private Picture picture;
        private ApplicationInfo applicationInfo;
        private VorbisComment vorbisComment;
        private CueSheet cueSheet;
        private SeekTable seekTable;

        /// <summary>
        /// Returns the StreamInfo metedata block of the loaded Flac file.
        /// </summary>
        public StreamInfo StreamInfo { get { return this.streamInfo; } }

        /// <summary>
        /// Returns the Picture metadata block of the loaded Flac file or null if this block is not available.
        /// </summary>
        public Picture Picture { get { return this.picture; } }
        
        /// <summary>
        /// Returns the ApplicationInfo metadata block of the loaded Flac file or null if this block is not available.
        /// </summary>
        public ApplicationInfo ApplicationInfo { get { return this.applicationInfo; } }

        /// <summary>
        /// Returns the VorbisComment metadata block of the loaded Flac file or null if this block is not available.
        /// </summary>
        public VorbisComment VorbisComment { get { return this.vorbisComment; } }

        /// <summary>
        /// Returns the CueSheet metadata block of the loaded Flac file or null if this block is not available.
        /// </summary>
        public CueSheet CueSheet { get { return this.cueSheet; } }

        /// <summary>
        /// Returns the SeekTable metadata block of the loaded Flac file or null if this block is not available.
        /// </summary>
        public SeekTable SeekTable { get { return this.seekTable; } }

        #endregion

        #region Writing

        public void Save()
        {
            if (String.IsNullOrEmpty(this.filePath))
            {
                throw new FlacLibSharpSaveNotSupportedException();
            }

            string bufferFile = Path.GetTempFileName();
            using (var fs = new FileStream(bufferFile, FileMode.Create))
            {
                // First write the magic flac bytes ...
                fs.Write(magicFlacMarker, 0, magicFlacMarker.Length);

                foreach (MetadataBlock block in this.Metadata)
                {
                    try
                    {
                        block.WriteBlockData(fs);
                    }
                    catch (NotImplementedException)
                    {
                        // Ignore for now (testing)
                    }
                }

                // Metadata is written back to the new file stream, now we can copy the rest of the frames
                byte[] dataBuffer = new byte[4096];
                this.dataStream.Seek(this.frameStart, SeekOrigin.Begin);

                int read = 0;
                do {
                    read = this.dataStream.Read(dataBuffer, 0, dataBuffer.Length);
                    fs.Write(dataBuffer, 0, read);
                } while (read > 0);
            }

            this.dataStream.Close();

            File.Delete(this.filePath);
            File.Move(bufferFile, this.filePath);
        }

        #endregion

        /// <summary>
        /// Releases the loaded flac file.
        /// </summary>
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
