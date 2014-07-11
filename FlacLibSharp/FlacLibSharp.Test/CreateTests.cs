using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FlacLibSharp.Test.Helpers;

namespace FlacLibSharp.Test
{
    [TestClass]
    public class CreateTests
    {
        private string origFile = @"Data\testfile5.flac";
        private string newFile = @"Data\testfile5_temp.flac";

        /// <summary>
        /// Will create and add a block of padding, save the file and re-open it.
        /// </summary>
        [TestMethod]
        public void CreatePadding()
        {
            uint emptyBitCount = 256;
            FileHelper.GetNewFile(origFile, newFile);

            using (FlacFile flac = new FlacFile(newFile))
            {
                // Adding some bits of padding
                Padding paddingBlock = new Padding();
                paddingBlock.EmptyBitCount = emptyBitCount;
                flac.Metadata.Add(paddingBlock);

                flac.Save();
            }

            using (FlacFile flac = new FlacFile(newFile))
            {
                Padding padding = flac.Padding;
                Assert.AreEqual<uint>(emptyBitCount, padding.EmptyBitCount);
            }
        }
    }
}
