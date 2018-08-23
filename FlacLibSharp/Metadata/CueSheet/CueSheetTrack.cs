using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// TODO: A single track in the cuesheet.
    /// </summary>
    public class CueSheetTrack {

        private const int ISRC_LENGTH = 12;
        private const int RESERVED_NULLDATA_LENGTH = 13;

        public CueSheetTrack()
        {
            // Initializing some "reasonable" defaults
            this.trackOffset = 0;
            this.trackNumber = 0;
            this.isrc = string.Empty;
            this.isAudioTrack = true;
            this.isPreEmphasis = false;

            this.indexPoints = new CueSheetTrackIndexCollection();
        }

        /// <summary>
        /// Initialize the CueSheetTrack.
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
            byte indexPointCount = (byte)BinaryDataHelper.GetUInt64(data, dataOffset + 35, 8);

            if (indexPointCount > 100)
            {
                throw new FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException(string.Format("CueSheet track nr {0} has an invalid Track Index Count of {1}. Maximum allowed is 100.", this.TrackNumber, indexPointCount));
            }

            // For all tracks, except the lead-in track, one or more track index points
            dataOffset += 36;
            for (int i = 0; i < indexPointCount; i++)
            {
                this.IndexPoints.Add(new CueSheetTrackIndex(data, dataOffset));
                dataOffset += 12; // Index points are always 12 bytes long
            }

            if (indexPointCount != this.IndexPoints.Count)
            {
                // Should we be so strict?
                throw new FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException(string.Format("CueSheet track nr {0} indicates {1} index points, but actually {2} index points are present.", this.TrackNumber, indexPointCount, this.IndexPoints.Count));
            }

        }

        /// <summary>
        /// Writes the data representing this CueSheet track to the given stream.
        /// </summary>
        /// <param name="targetStream"></param>
        public void WriteBlockData(Stream targetStream)
        {
            targetStream.Write(BinaryDataHelper.GetBytesUInt64(this.trackOffset), 0, 8);
            targetStream.WriteByte(this.TrackNumber);
            targetStream.Write(BinaryDataHelper.GetPaddedAsciiBytes(this.isrc, ISRC_LENGTH), 0, ISRC_LENGTH);
            
            byte trackAndEmphasis = 0;
            if (this.IsAudioTrack)
            {
                trackAndEmphasis += 0x80; // Most significant bit to 1
            }
            if (this.IsPreEmphasis)
            {
                trackAndEmphasis += 0x40; // Second most significant bit to 1
            }
            targetStream.WriteByte(trackAndEmphasis);

            byte[] nullData = new byte[RESERVED_NULLDATA_LENGTH];
            targetStream.Write(nullData, 0, nullData.Length);

            targetStream.WriteByte(this.IndexPointCount);

            foreach (var indexPoint in this.IndexPoints)
            {
                indexPoint.WriteBlockData(targetStream);
            }
        }

        private UInt64 trackOffset;

        /// <summary>
        /// Offset of a track.
        /// </summary>
        public UInt64 TrackOffset {
            get { return this.trackOffset; }
            set { this.trackOffset = value;  }
        }

        private byte trackNumber;

        /// <summary>
        /// Number of the track.
        /// </summary>
        public byte TrackNumber {
            get { return this.trackNumber; }
            set { this.trackNumber = value;  }
        }

        private string isrc;

        /// <summary>
        /// The ISRC of the track.
        /// </summary>
        public string ISRC {
            get { return this.isrc; }
            set { this.isrc = value; }
        }

        private bool isAudioTrack;

        /// <summary>
        /// Indicates whether or not this is an audio track.
        /// </summary>
        public bool IsAudioTrack {
            get { return this.isAudioTrack; }
            set { this.isAudioTrack = value; }
        }

        private bool isPreEmphasis;

        /// <summary>
        /// The Pre Emphasis flag.
        /// </summary>
        public bool IsPreEmphasis {
            get { return this.isPreEmphasis; }
            set { this.isPreEmphasis = value; }
        }

        /// <summary>
        /// Checks whether or not the track is a lead-out track (meaning track number is either 170 or 255, depending on CD-DA or not)
        /// </summary>
        public bool IsLeadOut
        {
            get
            {
                if (this.TrackNumber == CueSheet.CUESHEET_LEADOUT_TRACK_NUMBER_CDDA ||
                    this.trackNumber == CueSheet.CUESHEET_LEADOUT_TRACK_NUMBER_NON_CDDA)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Number of track index points. There must be at least one index in every track in a CUESHEET except for the lead-out track, which must have zero. For CD-DA, this number may be no more than 100.
        /// </summary>
        public byte IndexPointCount {
            get { return (byte)this.IndexPoints.Count; }
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
