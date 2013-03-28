using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp.Metadata {
    public class CueSheetTrackIndex {

        public CueSheetTrackIndex(byte[] data) {

        }

        private UInt64 offset;

        public UInt64 Offset {
            get { return this.offset; }
            set { this.offset = value; }
        }

        private byte indexPointNumber;

        public byte IndexPointNumber {
            get { return this.indexPointNumber; }
            set { this.indexPointNumber = value; }
        }	

    }
}
