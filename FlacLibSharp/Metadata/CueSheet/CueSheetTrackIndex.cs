using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// One index in a Cue Sheet Track.
    /// </summary>
    public class CueSheetTrackIndex {

        /// <summary>
        /// Creates a new Cue Sheet Track Index based on the binary data provided.
        /// </summary>
        /// <param name="data"></param>
        public CueSheetTrackIndex(byte[] data, int dataOffset) {
            this.offset = BinaryDataHelper.GetUInt64(data, dataOffset);
            this.indexPointNumber = (byte)BinaryDataHelper.GetUInt(data, dataOffset + 8, 8);
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
