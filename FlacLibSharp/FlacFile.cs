﻿using System;
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

        #endregion

        #region Reading

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
                    case MetadataBlockHeader.MetadataBlockType.Seektable:
                        this.seekTable = (SeekTable)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.VorbisComment:
                        this.vorbisComment = (VorbisComment)lastMetaDataBlock;
                        break;
                    case MetadataBlockHeader.MetadataBlockType.Padding:
                        this.padding = (Padding)lastMetaDataBlock;
                        break;
                }
            } while (!lastMetaDataBlock.Header.IsLastMetaDataBlock);

            if (!foundStreamInfo)
                throw new Exceptions.FlacLibSharpStreamInfoMissing();
            
            // Remember where the frame data starts
            frameStart = this.dataStream.Position;
        }

#endregion

        #region Quick metadata access

        /* Direct access to different meta data */

        private StreamInfo streamInfo;
        private ApplicationInfo applicationInfo;
        private VorbisComment vorbisComment;
        private CueSheet cueSheet;
        private SeekTable seekTable;
        private Padding padding;

        /// <summary>
        /// Returns the StreamInfo metedata block of the loaded Flac file.
        /// </summary>
        public StreamInfo StreamInfo { get { return this.streamInfo; } }
        
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

        /// <summary>
        /// Returns the Padding metadata block of the loaded Flac file or null if this block is not available.
        /// </summary>
        public Padding Padding { get { return this.padding; } }

        /// <summary>
        /// Will return all Picture blocks available in the Flac file, or an empty list of none are found.
        /// </summary>
        /// <returns></returns>
        public List<Picture> GetPictures()
        {
            List<Picture> result = new List<Picture>();

            foreach (MetadataBlock block in this.Metadata)
            {
                if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Picture)
                {
                    result.Add((Picture)block);
                }
            }

            return result;
        }

        #endregion

        #region Writing

        public void Save()
        {
            if (String.IsNullOrEmpty(this.filePath) || !this.dataStream.CanSeek)
            {
                throw new FlacLibSharpSaveNotSupportedException();
            }

            string bufferFile = Path.GetTempFileName();
            using (var fs = new FileStream(bufferFile, FileMode.Create))
            {
                // First write the magic flac bytes ...
                fs.Write(magicFlacMarker, 0, magicFlacMarker.Length);

                for (var i = 0; i < this.Metadata.Count; i++) {
                    MetadataBlock block = this.Metadata[i];

                    // We have to make sure to set the last metadata bit correctly.
                    if (i == this.Metadata.Count - 1)
                    {
                        block.Header.IsLastMetaDataBlock = true;
                    }
                    else
                    {
                        block.Header.IsLastMetaDataBlock = false;
                    }

                    try
                    {
                        var startLength = fs.Length;
                        block.WriteBlockData(fs);
                        var writtenBytes = fs.Length - startLength;

                        // minus 4 bytes because the MetaDataBlockLength excludes the size of the header
                        if (writtenBytes - 4 != block.Header.MetaDataBlockLength)
                        {
                            throw new ApplicationException(String.Format("The header of metadata block of type {0} claims a length of {1} bytes but the total amount of data written was {2} + 4 bytes",
                                block.Header.Type, block.Header.MetaDataBlockLength, writtenBytes));
                        }
                    }
                    catch (NotImplementedException)
                    {
                        // Ignore for now (testing) - we'll remove this handler later!
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
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up managed resources
                if (dataStream != null)
                {
                    dataStream.Dispose();
                    dataStream = null;
                }
            }
        }

     }
}
