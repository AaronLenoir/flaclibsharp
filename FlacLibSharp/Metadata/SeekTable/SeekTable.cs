using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A seektable.
    /// </summary>
    public class SeekTable : MetadataBlock {

        private SeekPointCollection seekPoints;

        /// <summary>
        /// Creates a new SeekTable base on the provided binary data.
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data) {
            int numberOfSeekpoints;
            SeekPoint newSeekPoint;

            numberOfSeekpoints = this.Header.MetaDataBlockLength / 18;
            for (int i = 0; i < numberOfSeekpoints; i++) {
                newSeekPoint = new SeekPoint(BinaryDataHelper.GetDataSubset(data, i * 18, 18));
                this.SeekPoints.Add(newSeekPoint);
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

        /// <summary>
        /// The seekpoints in the seektable.
        /// </summary>
        public SeekPointCollection SeekPoints {
            get {
                if (this.seekPoints == null) {
                    this.seekPoints = new SeekPointCollection();
                }
                return this.seekPoints;
            }
            set { this.seekPoints = value; }
        }

    }
}
