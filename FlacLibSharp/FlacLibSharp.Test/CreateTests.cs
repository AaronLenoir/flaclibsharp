﻿using System;
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
        
        /// <summary>
        /// Will create and add a picture metadata, save the file and re-open it.
        /// </summary>
        [TestMethod]
        public void CreatePicture()
        {
            uint colorDepth = 24;
            uint colors = 256;
            byte[] data = System.IO.File.ReadAllBytes(@"Data\testimage.png");
            string description = "Test Picture";
            uint height = 213;
            uint width = 400;
            PictureType pictureType = PictureType.LeadArtist;
            string mimeType = "image/jpeg";

            FileHelper.GetNewFile(origFile, newFile);

            using (FlacFile flac = new FlacFile(newFile))
            {
                Picture pictureBlock = new Picture();

                pictureBlock.ColorDepth = colorDepth;
                pictureBlock.Colors = colors;
                pictureBlock.Data = data;
                pictureBlock.Description = description;
                pictureBlock.Height = height;
                pictureBlock.Width = width;
                pictureBlock.PictureType = pictureType;
                pictureBlock.MIMEType = mimeType;

                flac.Metadata.Add(pictureBlock);

                flac.Save();
            }

            using (FlacFile flac = new FlacFile(newFile))
            {
                foreach (MetadataBlock block in flac.Metadata)
                {
                    if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Picture)
                    {
                        Picture pictureBlock = (Picture)block;

                        Assert.IsNotNull(pictureBlock);

                        Assert.AreEqual<uint>(colorDepth, pictureBlock.ColorDepth);
                        Assert.AreEqual<uint>(colors, pictureBlock.Colors);
                        Assert.AreEqual<string>(description, pictureBlock.Description);
                        Assert.AreEqual<uint>(height, pictureBlock.Height);
                        Assert.AreEqual<uint>(width, pictureBlock.Width);
                        Assert.AreEqual<PictureType>(pictureType, pictureBlock.PictureType);
                        Assert.AreEqual<string>(mimeType, pictureBlock.MIMEType);

                        bool dataIsSame = true;
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i] != pictureBlock.Data[i])
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
    }
}
