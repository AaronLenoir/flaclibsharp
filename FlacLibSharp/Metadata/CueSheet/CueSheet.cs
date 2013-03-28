using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp.Metadata {
    public class CueSheet : MetadataBlock {

        public override void LoadBlockData(byte[] data) {
            this.mediaCatalog = Encoding.ASCII.GetString(data, 0, 128);
            this.leadInSampleCount = BinaryDataHelper.GetUInt64(data, 128);
            this.isCDCueSheet = BinaryDataHelper.GetBoolean(data, 132, 0);
            // We're skipping 7 bits + 258 bytes which is reserved null data
            this.trackCount = data[391];
            for (int i = 0; i < this.trackCount; i++) {
                //this.tracks.Add(new FLACCueSheetTrack(BinaryDataHelper.GetDataSubset(data, 392 + (i*
            }
        }

        private string mediaCatalog;

        public string MediaCatalog {
            get { return this.mediaCatalog; }
            set { this.mediaCatalog = value; }
        }

        private UInt64 leadInSampleCount;

        public UInt64 LeadInSampleCount {
            get { return this.leadInSampleCount; }
            set { this.leadInSampleCount = value; }
        }

        private Boolean isCDCueSheet;

        public Boolean IsCDCueSheet {
            get { return this.isCDCueSheet; }
            set { this.isCDCueSheet = value; }
        }

        private byte trackCount;

        public byte TrackCount {
            get { return this.trackCount; }
            set { this.trackCount = value; }
        }

        private CueSheetTrackCollection tracks;

        public CueSheetTrackCollection Tracks {
            get {
                if (this.tracks == null) {
                    this.tracks = new CueSheetTrackCollection();
                }
                return this.tracks;
            }
            set { this.tracks = value; }
        }

    }
}
