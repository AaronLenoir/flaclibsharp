using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp.Metadata {
    public class SeekTable : MetadataBlock {

        private SeekPointCollection seekPoints;

        public override void LoadBlockData(byte[] data) {
            int numberOfSeekpoints;
            SeekPoint newSeekPoint;

            numberOfSeekpoints = this.Header.MetaDataBlockLength / 18;
            for (int i = 0; i < numberOfSeekpoints; i++) {
                newSeekPoint = new SeekPoint(BinaryDataHelper.GetDataSubset(data, i * 18, 18));
                this.SeekPoints.Add(newSeekPoint);
            }
        }

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
