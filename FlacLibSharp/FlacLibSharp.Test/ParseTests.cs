﻿using System;
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

        // testfile3.flac output from metadata --list 
        //seek points: 20
        //point 0: sample_number=0, stream_offset=0, frame_samples=4096
        //point 1: sample_number=86016, stream_offset=59718, frame_samples=4096
        //point 2: sample_number=176128, stream_offset=121744, frame_samples=4096
        //point 3: sample_number=262144, stream_offset=180590, frame_samples=4096
        //point 4: sample_number=352256, stream_offset=243068, frame_samples=4096
        //point 5: sample_number=438272, stream_offset=302152, frame_samples=4096
        //point 6: sample_number=528384, stream_offset=365119, frame_samples=4096
        //point 7: sample_number=614400, stream_offset=425558, frame_samples=4096
        //point 8: sample_number=704512, stream_offset=485065, frame_samples=4096
        //point 9: sample_number=790528, stream_offset=578047, frame_samples=4096
        //point 10: sample_number=880640, stream_offset=655947, frame_samples=4096
        //point 11: sample_number=966656, stream_offset=721964, frame_samples=4096
        //point 12: sample_number=1056768, stream_offset=793937, frame_samples=4096
        //point 13: sample_number=1142784, stream_offset=866865, frame_samples=4096
        //point 14: sample_number=1232896, stream_offset=942943, frame_samples=4096
        //point 15: sample_number=1318912, stream_offset=1033153, frame_samples=4096
        //point 16: sample_number=1409024, stream_offset=1138560, frame_samples=4096
        //point 17: sample_number=1499136, stream_offset=1215647, frame_samples=4096
        //point 18: sample_number=1585152, stream_offset=1321342, frame_samples=4096
        //point 19: sample_number=1675264, stream_offset=1451579, frame_samples=4096

        [TestMethod]
        public void OpenFlacFileAndCheckSeekTable()
        {
            using (FlacFile file = new FlacFile(@"Data\testfile3.flac"))
            {
                var seekTable = file.SeekTable;
                // There's a seekpoint every 2 seconds so there should be 20 seekpoints ...
                Assert.AreEqual(20, seekTable.SeekPoints.Count);
                // We know seekpoint 0 should start at sample number 0, with an offset of 0 and number of samples = 4096
                Assert.AreEqual(seekTable.SeekPoints.Values[0].FirstSampleNumber, (ulong)0);
                Assert.AreEqual(seekTable.SeekPoints.Values[0].ByteOffset, (ulong)0);
                Assert.AreEqual(seekTable.SeekPoints.Values[0].NumberOfSamples, (ushort)4096);
                // We know seekpoint 2 should start at sample number 176128, with an offset of 121744 and number of samples = 4096
                Assert.AreEqual(seekTable.SeekPoints.Values[2].FirstSampleNumber, (ulong)176128);
                Assert.AreEqual(seekTable.SeekPoints.Values[2].ByteOffset, (ulong)121744);
                Assert.AreEqual(seekTable.SeekPoints.Values[2].NumberOfSamples, (ushort)4096);
                // We know seekpoint 5 should start at sample number 438272, with an offset of 302152 and number of samples = 4096
                Assert.AreEqual(seekTable.SeekPoints.Values[5].FirstSampleNumber, (ulong)438272);
                Assert.AreEqual(seekTable.SeekPoints.Values[5].ByteOffset, (ulong)302152);
                Assert.AreEqual(seekTable.SeekPoints.Values[5].NumberOfSamples, (ushort)4096);
                // We know seekpoint 19 should start at sample number 1675264, with an offset of 1451579 and number of samples = 4096
                Assert.AreEqual(seekTable.SeekPoints.Values[19].FirstSampleNumber, (ulong)1675264);
                Assert.AreEqual(seekTable.SeekPoints.Values[19].ByteOffset, (ulong)1451579);
                Assert.AreEqual(seekTable.SeekPoints.Values[19].NumberOfSamples, (ushort)4096);
                // Not testing ALL seekpoints ...
            }
        }
      

    }
}
