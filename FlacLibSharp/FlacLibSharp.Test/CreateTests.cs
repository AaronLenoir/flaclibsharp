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


        /// <summary>
        /// Will create and add a block of application info, save the file and re-open it.
        /// </summary>
        [TestMethod]
        public void CreateApplicationInfo()
        {
            uint applicationID = 10;
            byte[] data = { 10, 20, 30, 40, 45 };

            FileHelper.GetNewFile(origFile, newFile);

            using (FlacFile flac = new FlacFile(newFile))
            {
                // Adding some bits of padding
                ApplicationInfo appInfoBlock = new ApplicationInfo();
                appInfoBlock.ApplicationID = 10;
                appInfoBlock.ApplicationData = data;
                flac.Metadata.Add(appInfoBlock);

                flac.Save();
            }

            using (FlacFile flac = new FlacFile(newFile))
            {
                ApplicationInfo appInfoBlock = flac.ApplicationInfo;
                Assert.IsNotNull(appInfoBlock);
                Assert.AreEqual<uint>(applicationID, appInfoBlock.ApplicationID);

                bool dataIsSame = true;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != appInfoBlock.ApplicationData[i])
                    {
                        dataIsSame = false;
                        break;
                    }
                }

                Assert.IsTrue(dataIsSame);
                
            }
        }
    }
}
