using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// The Flac metadata application block
    /// </summary>
    public class ApplicationInfo : MetadataBlock {

        private UInt32 applicationID;
        private byte[] applicationData;

        public override void LoadBlockData(byte[] data) {
            this.applicationID = BinaryDataHelper.GetUInt32(data, 0);
            this.applicationData = new byte[data.Length - 4];
            for (int i = 3; i < data.Length; i++) {
                this.applicationData[i - 3] = data[i];
            }
        }

        public UInt32 ApplicationID {
            get { return this.applicationID; }
            set { this.applicationID = value; }
        }

        public byte[] ApplicationData {
            get { return this.applicationData; }
            set { this.applicationData = value; }
        }

    }
}
