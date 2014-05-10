﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FlacLibSharp;

namespace FlacLibSharp.Test
{
    [TestClass]
    public class ParseTests
    {
        [TestMethod, TestCategory("ParseTests")]
        public void OpenAndCloseFlacFileWithFilePath()
        {
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                // Doing nothing
            }
        }

        [TestMethod, TestCategory("ParseTests")]
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

        [TestMethod, TestCategory("ParseTests")]
        [ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException), "Opening an invalid FLAC file was allowed.")]
        public void OpenInvalidFlacFile()
        {
            using (FlacFile file = new FlacFile(@"Data\noflacfile.ogg"))
            {
                // Doing nothing
            }
        }

        [TestMethod, TestCategory("ParseTests")]
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
        [TestMethod, TestCategory("ParseTests")]
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
                        Assert.AreEqual("1d2e54a059ea776787ef66f1f93d3e34", md5sum);
                        Assert.AreEqual(4096, info.MinimumBlockSize);
                        Assert.AreEqual(4096, info.MaximumBlockSize);
                        Assert.AreEqual<uint>(1427, info.MinimumFrameSize);
                        Assert.AreEqual<uint>(7211, info.MaximumFrameSize);
                        Assert.AreEqual<uint>(44100, info.SampleRateHz);
                        Assert.AreEqual(1, info.Channels);
                        Assert.AreEqual(16, info.BitsPerSample);
                        Assert.AreEqual(1703592, info.Samples);
                    }
                }
            }
        }

        [TestMethod, TestCategory("ParseTests")]
        public void OpenFlacFileAndCheckPadding()
        {
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                //Assert.IsTrue(file.Metadata.Count > 0, "No metadata blocks were found for the test file, this is not correct!");
                foreach (MetadataBlock block in file.Metadata)
                {
                    if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Padding)
                    {
                        Assert.AreEqual(block.Header.MetaDataBlockLength, ((Padding)block).EmptyBitCount / 8);
                    }
                }
            }
        }

        /// <summary>
        /// Will check some of the streaminfo as read from the testfile1.flac
        /// </summary>
        [TestMethod, TestCategory("ParseTests")]
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
                        Assert.AreEqual("Ziggystar", info["ARTIST"]);
                        Assert.AreEqual("Ziggystar", info.Artist);
                        Assert.AreEqual("Roland jx3p demo", info["TITLE"]);
                        Assert.AreEqual("Roland jx3p demo", info.Title);
                        Assert.AreEqual("Wiki Commons", info["ALBUM"]);
                        Assert.AreEqual("Wiki Commons", info.Album);
                        Assert.AreEqual("2005", info["DATE"]);
                        Assert.AreEqual("2005", info.Date);
                        Assert.AreEqual("01", info["TRACKNUMBER"]);
                        Assert.AreEqual("01", info.TrackNumber);
                        Assert.AreEqual("Electronic", info["GENRE"]);
                        Assert.AreEqual("Electronic", info.Genre);
                        Assert.IsFalse(info.ContainsField("UNEXISTINGKEY"));
                    }
                }
            }
        }

        /// <summary>
        /// Will check some of the streaminfo as read from the testfile1.flac
        /// </summary>
        [TestMethod, TestCategory("ParseTests")]
        public void CheckFastFlacFunctions()
        {
            Assert.AreEqual("Roland jx3p demo", FastFlac.GetTitle(@"Data\testfile1.flac"));
            Assert.AreEqual("Ziggystar", FastFlac.GetArtist(@"Data\testfile1.flac"));
            Assert.AreEqual("Wiki Commons", FastFlac.GetAlbum(@"Data\testfile1.flac"));
            Assert.AreEqual(38, FastFlac.GetDuration(@"Data\testfile1.flac"));
        }
        
        /// <summary>
        /// Will check some of the streaminfo as read from the testfile1.flac
        /// </summary>
        [TestMethod, TestCategory("ParseTests")]
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
                        Assert.AreEqual<UInt32>(213, info.Height);
                        Assert.AreEqual<UInt32>(400, info.Width);
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

        [TestMethod, TestCategory("ParseTests")]
        public void OpenFlacFileAndCheckSeekTable()
        {
            using (FlacFile file = new FlacFile(@"Data\testfile3.flac"))
            {
                var seekTable = file.SeekTable;
                // There's a seekpoint every 2 seconds so there should be 20 seekpoints ...
                Assert.AreEqual(20, seekTable.SeekPoints.Count);
                // We know seekpoint 0 should start at sample number 0, with an offset of 0 and number of samples = 4096
                Assert.AreEqual<ulong>(0, seekTable.SeekPoints.Values[0].FirstSampleNumber);
                Assert.AreEqual<ulong>(0, seekTable.SeekPoints.Values[0].ByteOffset);
                Assert.AreEqual<ulong>(4096, seekTable.SeekPoints.Values[0].NumberOfSamples);
                // We know seekpoint 2 should start at sample number 176128, with an offset of 121744 and number of samples = 4096
                Assert.AreEqual<ulong>(176128, seekTable.SeekPoints.Values[2].FirstSampleNumber);
                Assert.AreEqual<ulong>(121744, seekTable.SeekPoints.Values[2].ByteOffset);
                Assert.AreEqual<ushort>(4096, seekTable.SeekPoints.Values[2].NumberOfSamples);
                // We know seekpoint 5 should start at sample number 438272, with an offset of 302152 and number of samples = 4096
                Assert.AreEqual<ulong>(438272, seekTable.SeekPoints.Values[5].FirstSampleNumber);
                Assert.AreEqual<ulong>(302152, seekTable.SeekPoints.Values[5].ByteOffset);
                Assert.AreEqual<ushort>(4096, seekTable.SeekPoints.Values[5].NumberOfSamples);
                // We know seekpoint 19 should start at sample number 1675264, with an offset of 1451579 and number of samples = 4096
                Assert.AreEqual<ulong>(1675264, seekTable.SeekPoints.Values[19].FirstSampleNumber);
                Assert.AreEqual<ulong>(1451579, seekTable.SeekPoints.Values[19].ByteOffset);
                Assert.AreEqual<ushort>(4096, seekTable.SeekPoints.Values[19].NumberOfSamples);
                // Not testing ALL seekpoints ...
            }
        }

        //        METADATA block #2
        //  type: 5 (CUESHEET)
        //  is last: false
        //  length: 600
        //  media catalog number:
        //  lead-in: 88200
        //  is CD: false
        //  number of tracks: 4
        //    track[0]
        //      offset: 0
        //      number: 1
        //      ISRC:
        //      type: AUDIO
        //      pre-emphasis: false
        //      number of index points: 1
        //        index[0]
        //          offset: 0
        //          number: 1
        //    track[1]
        //      offset: 661500
        //      number: 2
        //      ISRC:
        //      type: AUDIO
        //      pre-emphasis: false
        //      number of index points: 2
        //        index[0]
        //          offset: 0
        //          number: 0
        //        index[1]
        //          offset: 220500
        //          number: 1
        //    track[2]
        //      offset: 1102500
        //      number: 3
        //      ISRC:
        //      type: AUDIO
        //      pre-emphasis: false
        //      number of index points: 2
        //        index[0]
        //          offset: 0
        //          number: 0
        //        index[1]
        //          offset: 220500
        //          number: 1
        //    track[3]
        //      offset: 1703592
        //      number: 170 (LEAD-OUT)

        [TestMethod, TestCategory("ParseTests")]
        public void OpenFlacFileAndCheckCueSheet()
        {
            using (FlacFile file = new FlacFile(@"Data\testfile4.flac"))
            {
                var cueSheet = file.CueSheet;
                Assert.IsNotNull(cueSheet, "No cuesheet found.");

                Assert.AreEqual<UInt32>(600, cueSheet.Header.MetaDataBlockLength);
                Assert.AreEqual<ulong>(88200, cueSheet.LeadInSampleCount);
                Assert.AreEqual(4, cueSheet.TrackCount);
                Assert.AreEqual(String.Empty, cueSheet.MediaCatalog);
                
                Assert.AreEqual(cueSheet.TrackCount, cueSheet.Tracks.Count);
                
                Assert.AreEqual<ulong>(0, cueSheet.Tracks[0].TrackOffset);
                Assert.AreEqual(1, cueSheet.Tracks[0].IndexPointCount);
                Assert.AreEqual(1, cueSheet.Tracks[0].TrackNumber);
                Assert.AreEqual(cueSheet.Tracks[0].IndexPoints.Count, cueSheet.Tracks[0].IndexPointCount);
                Assert.AreEqual(true, cueSheet.Tracks[0].IsAudioTrack);
                Assert.AreEqual(false, cueSheet.Tracks[0].IsPreEmphasis);
                Assert.AreEqual(String.Empty, cueSheet.Tracks[0].ISRC);

                Assert.AreEqual<ulong>(661500, cueSheet.Tracks[1].TrackOffset);
                Assert.AreEqual(2, cueSheet.Tracks[1].IndexPointCount);
                Assert.AreEqual(2, cueSheet.Tracks[1].TrackNumber);
                Assert.AreEqual(false, cueSheet.IsCDCueSheet);
                Assert.AreEqual(cueSheet.Tracks[1].IndexPoints.Count, cueSheet.Tracks[1].IndexPointCount, cueSheet.Tracks[1].IndexPoints.Count);
                Assert.AreEqual(true, cueSheet.Tracks[1].IsAudioTrack);
                Assert.AreEqual(false, cueSheet.Tracks[1].IsPreEmphasis);
                Assert.AreEqual(String.Empty, cueSheet.Tracks[1].ISRC);

                Assert.AreEqual<ulong>(1102500, cueSheet.Tracks[2].TrackOffset);
                Assert.AreEqual(2, cueSheet.Tracks[2].IndexPointCount);
                Assert.AreEqual(3, cueSheet.Tracks[2].TrackNumber);
                Assert.AreEqual(cueSheet.Tracks[2].IndexPoints.Count, cueSheet.Tracks[2].IndexPointCount);
                Assert.AreEqual(true, cueSheet.Tracks[2].IsAudioTrack);
                Assert.AreEqual(false, cueSheet.Tracks[2].IsPreEmphasis);
                Assert.AreEqual(String.Empty, cueSheet.Tracks[2].ISRC);

                Assert.AreEqual<ulong>(1703592, cueSheet.Tracks[3].TrackOffset);
                Assert.AreEqual(170, cueSheet.Tracks[3].TrackNumber); // Lead-out
            }
        }

    }
}
