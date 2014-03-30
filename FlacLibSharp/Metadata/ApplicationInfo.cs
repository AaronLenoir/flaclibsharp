using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// An application specific block of data.
    /// </summary>
    public class ApplicationInfo : MetadataBlock {

        private UInt32 applicationID;
        private byte[] applicationData;

        /// <summary>
        /// Parses the given binary metadata to an ApplicationInfo block
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data) {
            this.applicationID = BinaryDataHelper.GetUInt32(data, 0);
            this.applicationData = new byte[data.Length - 4];
            for (int i = 3; i < data.Length; i++) {
                this.applicationData[i - 3] = data[i];
            }
        }

        /// <summary>
        /// Will write the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            this.Header.WriteHeaderData(targetStream);

            targetStream.Write(BinaryDataHelper.GetBytesUInt32(this.applicationID), 0, 8);
            targetStream.Write(this.applicationData, 0, this.applicationData.Length);
        }

        /// <summary>
        /// The application ID of the application for which the data is intended
        /// </summary>
        public UInt32 ApplicationID {
            get { return this.applicationID; }
        }

        /// <summary>
        /// The additional data
        /// </summary>
        public byte[] ApplicationData {
            get { return this.applicationData; }
        }

    }
}
