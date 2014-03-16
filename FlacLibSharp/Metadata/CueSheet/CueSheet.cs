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

        /// <summary>
        /// TODO: Parses the binary metadata from the flac file into a CueSheet object.
        /// </summary>
        /// <param name="data">The binary data from the flac file.</param>
        public override void LoadBlockData(byte[] data) {
            this.mediaCatalog = Encoding.ASCII.GetString(data, 0, 128).Trim(new char[]{ '\0' });
            this.leadInSampleCount = BinaryDataHelper.GetUInt64(data, 128);
            this.isCDCueSheet = BinaryDataHelper.GetBoolean(data, 136, 0);
            // We're skipping 7 bits + 258 bytes which is reserved null data
            this.trackCount = data[395];
            if (this.trackCount > 100)
            {
                // Do we really need to throw an exception here?
                throw new Exceptions.FlacLibSharpInvalidFormatException("CueSheet has invalid track count. Cannot be more than 100.");
            }

            int cueSheetTrackOffset = 396;
            for (int i = 0; i < this.trackCount; i++) {
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
            throw new NotImplementedException();
        }

        private string mediaCatalog;

        /// <summary>
        /// The media catalog number.
        /// </summary>
        public string MediaCatalog {
            get { return this.mediaCatalog; }
        }

        private UInt64 leadInSampleCount;

        /// <summary>
        /// The number of lead-in samples, this field is only relevant for CD-DA cuesheets.
        /// </summary>
        public UInt64 LeadInSampleCount {
            get { return this.leadInSampleCount; }
        }

        private Boolean isCDCueSheet;

        /// <summary>
        /// True if the cuesheet corresponds to a Compact Disc.
        /// </summary>
        public Boolean IsCDCueSheet {
            get { return this.isCDCueSheet; }
        }

        private byte trackCount;

        /// <summary>
        /// The number of tracks.
        /// </summary>
        public byte TrackCount {
            get { return this.trackCount; }
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

    }
}
