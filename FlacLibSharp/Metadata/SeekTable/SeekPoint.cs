using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    public class SeekPoint : IComparable<SeekPoint> {

        private UInt64 firstSampleNumber;
        private UInt64 byteOffset;
        private UInt16 numberOfSamples;
        private bool isPlaceHolder;

        public SeekPoint(byte[] data) {
            this.firstSampleNumber = BinaryDataHelper.GetUInt64(data, 0);
            this.byteOffset = BinaryDataHelper.GetUInt64(data, 8);
            this.numberOfSamples = BinaryDataHelper.GetUInt16(data, 16);
            ValidateIsPlaceholder();
        }

        public SeekPoint() {
            this.firstSampleNumber = Int64.MaxValue;
            this.isPlaceHolder = true;
        }

        public UInt64 FirstSampleNumber {
            get { return this.firstSampleNumber; }
            set {
                this.firstSampleNumber = value;
                ValidateIsPlaceholder();
            }
        }

        public UInt64 ByteOffset {
            get { return this.byteOffset; }
            set { this.byteOffset = value; }
        }

        public UInt16 NumberOfSamples {
            get { return this.numberOfSamples; }
            set { this.numberOfSamples = value; }
        }

        public bool IsPlaceHolder {
            get { return this.isPlaceHolder; }
            set { this.isPlaceHolder = value; }
        }

        private void ValidateIsPlaceholder() {
            if (this.FirstSampleNumber == UInt64.MaxValue) {
                this.isPlaceHolder = true;
            } else {
                this.isPlaceHolder = false;
            }
        }

        #region IComparable<FLACSeekPoint> Members

        public int CompareTo(SeekPoint other) {
            if (this.firstSampleNumber == other.firstSampleNumber) {
                // We're equal... in the reality of FLAC this may never happen
                return 0;
            } else if (this.firstSampleNumber < other.firstSampleNumber) {
                // I precede the other, because my samplenumber is smaller
                return -1;
            } else {
                // I follow the other, because my samplenumber is smaller
                return 1;
            }
        }

        #endregion
    }
}
