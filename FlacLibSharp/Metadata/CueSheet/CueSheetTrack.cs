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
        /// <param name="data">The full data array.</param>
        /// <param name="dataOffset">Where the cuesheet track begins.</param>
        public CueSheetTrack(byte[] data, int dataOffset) {
            this.trackOffset = BinaryDataHelper.GetUInt64(data, dataOffset);
            this.trackNumber = (byte)BinaryDataHelper.GetUInt64(data, dataOffset + 8, 8);
            this.isrc = System.Text.Encoding.ASCII.GetString(data, dataOffset + 9, 12).Trim(new char[] { '\0' });
            this.isAudioTrack = !BinaryDataHelper.GetBoolean(data, dataOffset + 21, 1); // 0 for audio
            this.isPreEmphasis = BinaryDataHelper.GetBoolean(data, dataOffset + 21, 2);
            // 6 bits + 13 bytes need to be zero, won't check this
            this.indexPointCount = (byte)BinaryDataHelper.GetUInt64(data, dataOffset + 35, 8);

            // For all tracks, except the lead-in track, one or more track index points
            dataOffset += 36;
            for (int i = 0; i < indexPointCount; i++)
            {
                this.IndexPoints.Add(new CueSheetTrackIndex(data, dataOffset));
                dataOffset += 12; // Index points are always 12 bytes long
            }
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
            get {
                if (this.indexPoints == null) { 
                    this.indexPoints = new CueSheetTrackIndexCollection();
                }
                return this.indexPoints;
            }
            set { this.indexPoints = value; }
        }

    }
}
