using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A metadata block that contains information on the actual stream.
    /// </summary>
    public class StreamInfo : MetadataBlock {

        #region Private Fields

        private UInt16 minimumBlockSize;
        private UInt16 maximumBlockSize;
        // Is actually an Int24 but we won't bother creating that type now... maybe later
        private UInt32 minimumFrameSize;
        private UInt32 maximumFrameSize;
        // Is actually 20 bit: "Sample rate in Hz. Though 20 bits are available, the maximum sample rate is limited by the structure of frame headers to 655350Hz. Also, a value of 0 is invalid."
        private UInt32 sampleRateHz;
        // 3-bit value: maximum 8 channels
        private Int16 channels;
        // 5-bit value: 4 to 32 bits per sample
        private Int16 bitsPerSample;
        // 36 byte (!) value: 
        private Int64 samples;
        // Should contain 16 bytes...
        private byte[] md5Signature;

        #endregion

        /// <summary>
        /// Loads a new stream info block from the provided data.
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data) {
            //throw new Exception("The method or operation is not implemented.");

            // "All numbers are big-endian coded and unsigned".

            // 1: Minimum Block Size (first 16-bit)
            this.minimumBlockSize = BinaryDataHelper.GetUInt16(data, 0);
            this.maximumBlockSize = BinaryDataHelper.GetUInt16(data, 2);
            this.minimumFrameSize = BinaryDataHelper.GetUInt24(data, 4);
            this.maximumFrameSize = BinaryDataHelper.GetUInt24(data, 7);
            // Interpret 20 bits starting from byte 10 as a UInt
            this.sampleRateHz = (UInt32)BinaryDataHelper.GetUInt64(data, 10, 20);
            this.channels = (short)(BinaryDataHelper.GetUInt64(data, 12, 3, 4) + 1);
            this.bitsPerSample = (short)(BinaryDataHelper.GetUInt64(data, 12, 5, 7) + 1);
            this.samples = (long)BinaryDataHelper.GetUInt64(data, 13, 36, 4);
            this.md5Signature = new byte[16];
            Array.Copy(data, 18, this.md5Signature, 0, 16);
        }

        /// <summary>
        /// When overridden in a derived class, will write the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            this.Header.WriteHeaderData(targetStream);

            targetStream.Write(BinaryDataHelper.GetBytesUInt16(this.minimumBlockSize), 0, 2);
            targetStream.Write(BinaryDataHelper.GetBytesUInt16(this.maximumBlockSize), 0, 2);
            targetStream.Write(BinaryDataHelper.GetBytes(this.minimumFrameSize, 3), 0, 3);
            targetStream.Write(BinaryDataHelper.GetBytes(this.maximumFrameSize, 3), 0, 3);

            // next are 64 bits containing:
            // * sample rate in Hz (20-bits)
            // * number of channels (3 bits)
            // * bits per sample (5 bits)
            // * total samples per stream (36 bits)
            UInt64 combinedData = (UInt64)this.SampleRateHz << 44;
            combinedData = combinedData + ((UInt64)(this.channels-1) << 41);
            combinedData = combinedData + ((UInt64)(this.bitsPerSample-1) << 36);
            combinedData = combinedData + (UInt64)this.samples;

            targetStream.Write(BinaryDataHelper.GetBytes(combinedData, 8), 0, 8);
            targetStream.Write(this.md5Signature, 0, 16);
        }

        #region Public Properties

        /// <summary>
        /// The minimum block size (in samples) used in the stream.
        /// </summary>
        public UInt16 MinimumBlockSize {
            get { return this.minimumBlockSize; }
        }

        /// <summary>
        /// The maximum block size (in samples) used in the stream. (Minimum blocksize == maximum blocksize) implies a fixed-blocksize stream.
        /// </summary>
        public UInt16 MaximumBlockSize {
            get { return this.maximumBlockSize; }
        }

        /// <summary>
        /// The minimum frame size (in bytes) used in the stream. May be 0 to imply the value is not known.
        /// </summary>
        public UInt32 MinimumFrameSize {
            get { return this.minimumFrameSize; }
        }

        /// <summary>
        /// The maximum frame size (in bytes) used in the stream. May be 0 to imply the value is not known.
        /// </summary>
        public UInt32 MaximumFrameSize {
            get { return this.maximumFrameSize; }
        }

        /// <summary>
        /// Sample rate in Hz. Though 20 bits are available, the maximum sample rate is limited by the structure of frame headers to 655350Hz. Also, a value of 0 is invalid.
        /// </summary>
        public UInt32 SampleRateHz {
            get { return this.sampleRateHz; }
        }

        /// <summary>
        /// (number of channels)-1. FLAC supports from 1 to 8 channels
        /// </summary>
        public Int16 Channels {
            get { return this.channels; }
        }

        /// <summary>
        /// (bits per sample)-1. FLAC supports from 4 to 32 bits per sample. Currently the reference encoder and decoders only support up to 24 bits per sample.
        /// </summary>
        public Int16 BitsPerSample {
            get { return this.bitsPerSample; }
        }
        
        /// <summary>
        /// Total samples in stream. 'Samples' means inter-channel sample, i.e. one second of 44.1Khz audio will have 44100 samples regardless of the number of channels. A value of zero here means the number of total samples is unknown.
        /// </summary>
        public Int64 Samples {
            get { return this.samples; }
        }

        /// <summary>
        /// MD5 signature (16 byte) of the unencoded audio data. This allows the decoder to determine if an error exists in the audio data even when the error does not result in an invalid bitstream.
        /// </summary>
        public byte[] MD5Signature {
            get { return this.md5Signature; }
        }

        /// <summary>
        /// The duration of the audio in seconds, calculated based on the stream info.
        /// </summary>
        public int Duration
        {
            get
            {
                return (int)Math.Round((double)(this.Samples / this.SampleRateHz));
            }
        }

        #endregion
	
    }
}
