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

        private const uint SEEKPOINT_SIZE = 18;

        private SeekPointCollection seekPoints;

        public SeekTable() : base() {
            this.Header.Type = MetadataBlockHeader.MetadataBlockType.Seektable;
            // An empty SeekTable contains no seekpoints and seekspoints are all this metadata block has
            this.Header.MetaDataBlockLength = 0;
        }

        /// <summary>
        /// Creates a new SeekTable base on the provided binary data.
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data) {
            UInt32 numberOfSeekpoints;
            SeekPoint newSeekPoint;

            numberOfSeekpoints = this.Header.MetaDataBlockLength / SEEKPOINT_SIZE;
            for (int i = 0; i < numberOfSeekpoints; i++) {
                newSeekPoint = new SeekPoint(BinaryDataHelper.GetDataSubset(data, i * (int)SEEKPOINT_SIZE, (int)SEEKPOINT_SIZE));
                // We should keep in mind that the placeholder seekpoints aren't actually added to the list but are kept only as a
                // count in this.SeekPoints.Placeholders
                this.SeekPoints.Add(newSeekPoint);
            }
        }

        /// <summary>
        /// Will write the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            this.Header.MetaDataBlockLength = (uint)(SeekPoints.Count + SeekPoints.Placeholders) * SEEKPOINT_SIZE;
            this.Header.WriteHeaderData(targetStream);

            foreach (var seekPoint in this.SeekPoints)
            {
                seekPoint.Value.WriteData(targetStream);
            }

            SeekPoint placeholder = new SeekPoint();
            placeholder.FirstSampleNumber = ulong.MaxValue;
            for (int i = 0; i < SeekPoints.Placeholders; i++)
            {
                // Here, we shall actually physically write placeholders, even though they don't really exist in memory ...
                // Again: this is a weird unnatural way to support multiple placeholders and it should be changed at some point!
                placeholder.WriteData(targetStream);
            }
        }

        /// <summary>
        /// Gets the total number of SeekPoints (normal seekpoints + placeholder seekpoints).
        /// </summary>
        /// <remarks>Don't use this to loop through the indexes of the SeekPoints collection! It also contains the total nr of placeholders, which are not in that collection.</remarks>
        public int TotalSeekPoints {
            get { return this.SeekPoints.Count + this.SeekPoints.Placeholders; }
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
        }

    }
}
