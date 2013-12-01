using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// TODO: A single track in the cuesheet.
    /// </summary>
    public class CueSheetTrack {

        /// <summary>
        /// Initialize the CueSheetTrack
        /// </summary>
        /// <param name="data"></param>
        public CueSheetTrack(byte[] data) {

        }

        private UInt64 trackOffset;

        /// <summary>
        /// Offset of a track.
        /// </summary>
        public UInt64 TrackOffset {
            get { return this.trackOffset; }
        }

        private byte trackNumber;

        /// <summary>
        /// Number of the track.
        /// </summary>
        public byte TrackNumber {
            get { return this.trackNumber; }
        }

        private string isrc;

        /// <summary>
        /// The ISRC of the track.
        /// </summary>
        public string ISRC {
            get { return this.isrc; }
        }

        private bool isAudioTrack;

        /// <summary>
        /// Indicates whether or not this is an audio track.
        /// </summary>
        public bool IsAudioTrack {
            get { return this.isAudioTrack; }
        }

        private bool isPreEmphasis;

        /// <summary>
        /// The Pre Emphasis flag.
        /// </summary>
        public bool IsPreEmphasis {
            get { return this.isPreEmphasis; }
            set { this.isPreEmphasis = value; }
        }

        private byte indexPointCount;

        /// <summary>
        /// Number of track index points. There must be at least one index in every track in a CUESHEET except for the lead-out track, which must have zero. For CD-DA, this number may be no more than 100.
        /// </summary>
        public byte IndexPointCount {
            get { return this.indexPointCount; }
            set { this.indexPointCount = value; }
        }

        private CueSheetTrackIndexCollection indexPoints;

        /// <summary>
        /// All of the index points in the cue sheet track.
        /// </summary>
        public CueSheetTrackIndexCollection IndexPoints {
            get { return this.indexPoints; }
            set { this.indexPoints = value; }
        }

    }
}
