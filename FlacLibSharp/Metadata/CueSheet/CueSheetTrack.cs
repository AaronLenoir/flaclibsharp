using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp.Metadata {
    public class CueSheetTrack {

        public CueSheetTrack(byte[] data) {

        }

        private UInt64 trackOffset;

        public UInt64 TrackOffset {
            get { return this.trackOffset; }
            set { this.trackOffset = value; }
        }

        private byte trackNumber;

        public byte TrackNumber {
            get { return this.trackNumber; }
            set { this.trackNumber = value; }
        }

        private string isrc;

        public string ISRC {
            get { return this.isrc; }
            set { this.isrc = value; }
        }

        private bool isAudioTrack;

        public bool IsAudioTrack {
            get { return this.isAudioTrack; }
            set { this.isAudioTrack = value; }
        }

        private bool isPreEmphasis;

        public bool MyProperty {
            get { return this.isPreEmphasis; }
            set { this.isPreEmphasis = value; }
        }

        private byte indexPointCount;

        public byte IndexPointCount {
            get { return this.indexPointCount; }
            set { this.indexPointCount = value; }
        }

        private CueSheetTrackIndexCollection indexPoints;

        public CueSheetTrackIndexCollection IndexPoints {
            get { return this.indexPoints; }
            set { this.indexPoints = value; }
        }

    }
}
