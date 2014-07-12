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
        /// Creates an empty ApplicationInfo block, application id will be 0, application data will be empty.
        /// </summary>
        public ApplicationInfo()
        {
            this.Header.Type = MetadataBlockHeader.MetadataBlockType.Application;
            this.applicationID = 0;
            this.applicationData = new byte[0];
        }

        /// <summary>
        /// Parses the given binary metadata to an ApplicationInfo block
        /// </summary>
        /// <param name="data"></param>
        public override void LoadBlockData(byte[] data) {
            this.applicationID = BinaryDataHelper.GetUInt32(data, 0);
            this.applicationData = new byte[data.Length - 4];

            for (int i = 0; i < this.applicationData.Length; i++)
            {
                this.applicationData[i] = data[i + 4]; // + 4 because the first four bytes are the application ID!
            }
        }

        /// <summary>
        /// Will write the data describing this metadata block to the given stream.
        /// </summary>
        /// <param name="targetStream">Stream to write the data to.</param>
        public override void WriteBlockData(Stream targetStream)
        {
            this.Header.MetaDataBlockLength = 4 + (uint)this.applicationData.Length;
            this.Header.WriteHeaderData(targetStream);

            targetStream.Write(BinaryDataHelper.GetBytesUInt32(this.applicationID), 0, 4);
            targetStream.Write(this.applicationData, 0, this.applicationData.Length);
        }

        /// <summary>
        /// The application ID of the application for which the data is intended
        /// </summary>
        public UInt32 ApplicationID {
            get { return this.applicationID; }
            set { this.applicationID = value; }
        }

        /// <summary>
        /// The additional data
        /// </summary>
        public byte[] ApplicationData {
            get { return this.applicationData; }
            set { this.applicationData = value; }
        }

    }
}
