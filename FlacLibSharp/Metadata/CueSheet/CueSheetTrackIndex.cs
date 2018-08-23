using System;
using System.IO;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// One index in a Cue Sheet Track.
    /// </summary>
    public class CueSheetTrackIndex {

        private const int RESERVED_NULLDATA_LENGTH = 3;

        /// <summary>
        /// Creates a new Cue Sheet Track Index.
        /// </summary>
        public CueSheetTrackIndex()
        { }

        /// <summary>
        /// Creates a new Cue Sheet Track Index based on the binary data provided.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataOffset">Where in the data array to start reading.</param>
        public CueSheetTrackIndex(byte[] data, int dataOffset) {
            this.offset = BinaryDataHelper.GetUInt64(data, dataOffset);
            this.indexPointNumber = (byte)BinaryDataHelper.GetUInt64(data, dataOffset + 8, 8);
        }

        /// <summary>
        /// Writes the data representing this CueSheet track index point to the given stream.
        /// </summary>
        /// <param name="targetStream"></param>
        public void WriteBlockData(Stream targetStream)
        {
            targetStream.Write(BinaryDataHelper.GetBytesUInt64(this.offset), 0, 8);
            targetStream.WriteByte(this.indexPointNumber);

            byte[] nullData = new byte[RESERVED_NULLDATA_LENGTH];
            targetStream.Write(nullData, 0, nullData.Length);
        }

        private UInt64 offset;

        /// <summary>
        /// Offset in samples, relative to the track offset, of the index point. For CD-DA, the offset must be evenly divisible by 588 samples (588 samples = 44100 samples/sec * 1/75th of a sec). Note that the offset is from the beginning of the track, not the beginning of the audio data.
        /// </summary>
        public UInt64 Offset {
            get { return this.offset; }
            set { this.offset = value; }
        }

        private byte indexPointNumber;

        /// <summary>
        /// The index point number. For CD-DA, an index number of 0 corresponds to the track pre-gap. The first index in a track must have a number of 0 or 1, and subsequently, index numbers must increase by 1. Index numbers must be unique within a track.
        /// </summary>
        public byte IndexPointNumber {
            get { return this.indexPointNumber; }
            set { this.indexPointNumber = value; }
        }	

    }
}
