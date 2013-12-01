using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A seek point in a frame.
    /// </summary>
    public class SeekPoint : IComparable<SeekPoint> {

        private UInt64 firstSampleNumber;
        private UInt64 byteOffset;
        private UInt16 numberOfSamples;
        private bool isPlaceHolder;

        /// <summary>
        /// Creates a new seekpoint.
        /// </summary>
        /// <param name="data"></param>
        public SeekPoint(byte[] data) {
            this.firstSampleNumber = BinaryDataHelper.GetUInt64(data, 0);
            this.byteOffset = BinaryDataHelper.GetUInt64(data, 8);
            this.numberOfSamples = BinaryDataHelper.GetUInt16(data, 16);
            ValidateIsPlaceholder();
        }

        /// <summary>
        /// Creates a place holder seekpoint.
        /// </summary>
        public SeekPoint() {
            this.firstSampleNumber = Int64.MaxValue;
            this.isPlaceHolder = true;
        }

        /// <summary>
        /// Sample number of the first sample in a target frame, 0xFFFFFFFFFFFFFFFF for a placeholder point.
        /// </summary>
        public UInt64 FirstSampleNumber {
            get { return this.firstSampleNumber; }
            set {
                this.firstSampleNumber = value;
                ValidateIsPlaceholder();
            }
        }

        /// <summary>
        /// Offset (in bytes) from the first byte of the first frame header to the first byte of the target frame's header.
        /// </summary>
        public UInt64 ByteOffset {
            get { return this.byteOffset; }
            set { this.byteOffset = value; }
        }

        /// <summary>
        /// Number of samples in the target frame.
        /// </summary>
        public UInt16 NumberOfSamples {
            get { return this.numberOfSamples; }
            set { this.numberOfSamples = value; }
        }

        /// <summary>
        /// Indicates if this seekpoint is a place holder.
        /// </summary>
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
        
        /// <summary>
        /// Compares two seekpoints based on the "first sample number".
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
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
