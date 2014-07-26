using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// This block stores various information for use in a cue sheet.
    /// </summary>
    public class CueSheet : MetadataBlock {

        // See spec for details
        private const byte CUESHEET_LEADOUT_TRACK_NUMBER = 170;
        private const uint CUESHEET_BLOCK_DATA_LENGTH = 396;
        private const uint CUESHEET_TRACK_LENGTH = 36;
        private const uint CUESHEET_TRACK_INDEXPOINT_LENGTH = 12;
        private const int MEDIACATALOG_MAX_LENGTH = 128;
        private const int RESERVED_NULLDATA_LENGTH = 258;

        public CueSheet()
        {
            this.Header.Type = MetadataBlockHeader.MetadataBlockType.CueSheet;

            CalculateMetaDataBlockLength();
        }

        /// <summary>
        /// Parses the binary metadata from the flac file into a CueSheet object.
        /// </summary>
        /// <param name="data">The binary data from the flac file.</param>
        public override void LoadBlockData(byte[] data) {
            this.mediaCatalog = Encoding.ASCII.GetString(data, 0, 128).Trim(new char[]{ '\0' });
            this.leadInSampleCount = BinaryDataHelper.GetUInt64(data, 128);
            this.isCDCueSheet = BinaryDataHelper.GetBoolean(data, 136, 0);
            // We're skipping 7 bits + 258 bytes which is reserved null data
            byte trackCount = data[395];
            if (trackCount > 100)
            {
                // Do we really need to throw an exception here?
                throw new Exceptions.FlacLibSharpInvalidFormatException(string.Format("CueSheet has invalid track count {0}. Cannot be more than 100.", trackCount));
            }

            int cueSheetTrackOffset = 396;
            for (int i = 0; i < trackCount; i++)
            {
                CueSheetTrack newTrack = new CueSheetTrack(data, cueSheetTrackOffset);
                cueSheetTrackOffset += 36 + (12 * newTrack.IndexPointCount); // 36 bytes for the cueSheetTrack and 12 bytes per index point ...
                this.Tracks.Add(newTrack);
            }
        }

        /// <summary>
        /// Will write the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            if (this.Tracks.Count > 0)
            {
                var lastTrack = this.Tracks[this.Tracks.Count - 1];
                if (lastTrack.TrackNumber != CUESHEET_LEADOUT_TRACK_NUMBER)
                {
                    throw new FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException(string.Format("CueSheet is invalid, last track (nr {0}) is not the lead-out track.", lastTrack.TrackNumber));
                }
            }
            else
            {
                throw new FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException("CueSheet is invalid as it has no tracks, it must have at least one track (the lead-out track).");
            }

            // TODO: this value in the header should also update when someone add/removes tracks or track index points ...
            this.Header.MetaDataBlockLength = CalculateMetaDataBlockLength();
            this.Header.WriteHeaderData(targetStream);

            targetStream.Write(BinaryDataHelper.GetPaddedAsciiBytes(this.MediaCatalog, MEDIACATALOG_MAX_LENGTH), 0, MEDIACATALOG_MAX_LENGTH);
            targetStream.Write(BinaryDataHelper.GetBytesUInt64(this.LeadInSampleCount), 0, 8);
            
            byte isCDCueSheet = 0;
            if(this.isCDCueSheet) {
                isCDCueSheet = 0x80; // Most significant bit should be 1
            }
            targetStream.WriteByte(isCDCueSheet);

            // Now we need to write 258 bytes of 0 data ("reserved")
            byte[] nullData = new byte[RESERVED_NULLDATA_LENGTH];
            targetStream.Write(nullData, 0, nullData.Length);

            // The number of tracks i 1 byte in size ...
            targetStream.WriteByte(this.TrackCount);

            foreach (var track in Tracks)
            {
                track.WriteBlockData(targetStream);
            }

        }

        private string mediaCatalog;

        /// <summary>
        /// Gets or sets the media catalog number.
        /// </summary>
        public string MediaCatalog {
            get { return this.mediaCatalog; }
            set { this.mediaCatalog = value; }
        }

        private UInt64 leadInSampleCount;

        /// <summary>
        /// Gets or sets the number of lead-in samples, this field is only relevant for CD-DA cuesheets.
        /// </summary>
        public UInt64 LeadInSampleCount {
            get { return this.leadInSampleCount; }
            set { this.leadInSampleCount = value; }
        }

        private Boolean isCDCueSheet;

        /// <summary>
        /// Gets or sets whether the cuesheet corresponds to a Compact Disc.
        /// </summary>
        public Boolean IsCDCueSheet {
            get { return this.isCDCueSheet; }
            set { this.isCDCueSheet = value; }
        }

        /// <summary>
        /// The number of tracks.
        /// </summary>
        public byte TrackCount {
            get {
                return (byte)this.Tracks.Count;
            }
        }

        private CueSheetTrackCollection tracks;

        /// <summary>
        /// The collection of tracks in the cuesheet.
        /// </summary>
        public CueSheetTrackCollection Tracks {
            get {
                if (this.tracks == null) {
                    this.tracks = new CueSheetTrackCollection();
                }
                return this.tracks;
            }
        }

        /// <summary>
        /// Calculates the total Block Length of this metadata block (for use in the Header)
        /// </summary>
        /// <returns></returns>
        private uint CalculateMetaDataBlockLength()
        {
            uint totalLength = CUESHEET_BLOCK_DATA_LENGTH; // See the specs ...
            // The length of this metadata block is: 
            // 396 bytes for the CueSheet block data itself (see spec for details)
            // + 36 bytes per CueSheetTrack
            // + 12 bytes per CueSheetTrackIndex
            foreach (var track in this.Tracks)
            {
                totalLength += CUESHEET_TRACK_LENGTH + (track.IndexPointCount * CUESHEET_TRACK_INDEXPOINT_LENGTH);
            }

            return totalLength;
        }

    }
}
