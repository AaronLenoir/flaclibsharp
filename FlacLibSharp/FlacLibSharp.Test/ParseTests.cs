using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FlacLibSharp;

namespace FlacLibSharp.Test
{
    [TestClass]
    public class ParseTests
    {
        [TestMethod]
        public void OpenAndCloseFlacFileWithFilePath()
        {
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                // Doing nothing
            }
        }

        [TestMethod]
        public void OpenAndCloseFlacFileWithStream()
        {
            using (Stream stream = File.OpenRead(@"Data\testfile1.flac"))
            {
                using (FlacFile file = new FlacFile(stream))
                {
                    // Doing nothing
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException), "Opening an invalid FLAC file was allowed.")]
        public void OpenInvalidFlacFile()
        {
            using (FlacFile file = new FlacFile(@"Data\noflacfile.ogg"))
            {
                // Doing nothing
            }
        }

        [TestMethod]
        public void OpenFlacFileAndCheckMetadata()
        {
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                Assert.IsTrue(file.Metadata.Count > 0, "No metadata blocks were found for the test file, this is not correct!");
            }

        }

        /// <summary>
        /// Will check some of the streaminfo as read from the testfile1.flac
        /// </summary>
        [TestMethod]
        public void OpenFlacFileAndCheckStreamInfo()
        {

            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                //Assert.IsTrue(file.Metadata.Count > 0, "No metadata blocks were found for the test file, this is not correct!");
                foreach (MetadataBlock block in file.Metadata)
                {
                    if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.StreamInfo)
                    {
                        StreamInfo info = (StreamInfo)block;
                        string md5sum = Helpers.ByteHelper.ByteArrayToString(info.MD5Signature);
                        Assert.AreEqual(md5sum, "1d2e54a059ea776787ef66f1f93d3e34");
                        Assert.AreEqual(info.MinimumBlockSize, 4096);
                        Assert.AreEqual(info.MaximumBlockSize, 4096);
                        Assert.AreEqual(info.MinimumFrameSize, (uint)1427);
                        Assert.AreEqual(info.MaximumFrameSize, (uint)7211);
                        Assert.AreEqual(info.SampleRateHz, (uint)44100);
                        Assert.AreEqual(info.Channels, 1);
                        Assert.AreEqual(info.BitsPerSample, 16);
                        Assert.AreEqual(info.Samples, 1703592);
                    }
                }
            }
        }

        /// <summary>
        /// Will check some of the streaminfo as read from the testfile1.flac
        /// </summary>
        [TestMethod]
        public void OpenFlacFileAndCheckVorbisComment()
        {

            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                //Assert.IsTrue(file.Metadata.Count > 0, "No metadata blocks were found for the test file, this is not correct!");
                foreach (MetadataBlock block in file.Metadata)
                {
                    if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.VorbisComment)
                    {
                        VorbisComment info = (VorbisComment)block;
                        Assert.AreEqual(info["ARTIST"], "Ziggystar");
                        Assert.AreEqual(info.Artist, "Ziggystar");
                        Assert.AreEqual(info["TITLE"], "Roland jx3p demo");
                        Assert.AreEqual(info.Title, "Roland jx3p demo");
                        Assert.AreEqual(info["ALBUM"], "Wiki Commons");
                        Assert.AreEqual(info.Album, "Wiki Commons");
                        Assert.AreEqual(info["DATE"], "2005");
                        Assert.AreEqual(info.Date, "2005");
                        Assert.AreEqual(info["TRACKNUMBER"], "01");
                        Assert.AreEqual(info.TrackNumber, "01");
                        Assert.AreEqual(info["GENRE"], "Electronic");
                        Assert.AreEqual(info.Genre, "Electronic");
                        Assert.IsFalse(info.ContainsField("UNEXISTINGKEY"));
                    }
                }
            }
        }

        /// <summary>
        /// Will check some of the streaminfo as read from the testfile1.flac
        /// </summary>
        [TestMethod]
        public void CheckFastFlacFunctions()
        {
            Assert.AreEqual(FastFlac.GetTitle(@"Data\testfile1.flac"), "Roland jx3p demo");
            Assert.AreEqual(FastFlac.GetArtist(@"Data\testfile1.flac"), "Ziggystar");
            Assert.AreEqual(FastFlac.GetAlbum(@"Data\testfile1.flac"), "Wiki Commons");
            Assert.AreEqual(FastFlac.GetDuration(@"Data\testfile1.flac"), 38);
        }
        
        /// <summary>
        /// Will check some of the streaminfo as read from the testfile1.flac
        /// </summary>
        [TestMethod]
        public void OpenFlacFileAndCheckPicture()
        {

            using (FlacFile file = new FlacFile(@"Data\testfile2.flac"))
            {
                //Assert.IsTrue(file.Metadata.Count > 0, "No metadata blocks were found for the test file, this is not correct!");
                foreach (MetadataBlock block in file.Metadata)
                {
                    if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Picture)
                    {
                        Picture info = (Picture)block;
                        Assert.AreEqual(info.Height, (UInt32)213);
                        Assert.AreEqual(info.Width, (UInt32)400);
                        Assert.AreEqual(info.PictureType, PictureType.CoverFront);
                        Assert.AreEqual(info.MIMEType, "image/jpeg");
                    }
                }
            }
        }

    }
}
