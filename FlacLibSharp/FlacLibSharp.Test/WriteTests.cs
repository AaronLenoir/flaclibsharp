using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

using FlacLibSharp.Test.Helpers;

namespace FlacLibSharp.Test
{
    [TestClass]
    public class WriteTests
    {

        [TestMethod, TestCategory("Write Tests")]
        public void CopyOpenAndSaveStreamInfo()
        {
            string origFile = @"Data\testfile1.flac";
            string newFile = @"Data\testfile1_temp.flac";
            
            FileHelper.GetNewFile(origFile, newFile);

            string newArtist = String.Empty;
            string newTitle = String.Empty;

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    // Save flac file
                    flac.Save();
                }
                using (FlacFile flac = new FlacFile(newFile))
                {
                    // This will check whether the save did correctly write back the streaminfo
                    Assert.IsNotNull(flac.StreamInfo);
                    var info = flac.StreamInfo;
                    string md5sum = Helpers.ByteHelper.ByteArrayToString(info.MD5Signature);
                    Assert.AreEqual("1d2e54a059ea776787ef66f1f93d3e34", md5sum);
                    Assert.AreEqual(4096, info.MinimumBlockSize);
                    Assert.AreEqual(4096, info.MaximumBlockSize);
                    Assert.AreEqual((uint)1427, info.MinimumFrameSize);
                    Assert.AreEqual((uint)7211, info.MaximumFrameSize);
                    Assert.AreEqual((uint)44100, info.SampleRateHz);
                    Assert.AreEqual(1, info.Channels);
                    Assert.AreEqual(16, info.BitsPerSample);
                    Assert.AreEqual(1703592, info.Samples);
                }
            }
            finally
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
            }
        }

        [TestMethod, TestCategory("Write Tests")]
        public void CopyOpenEditAndSavePadding()
        {
            UInt32 newPaddingSize = 8 * 2; // 2 bytes of padding

            string origFile = @"Data\testfile1.flac";
            string newFile = @"Data\testfile1_temp.flac";

            FileHelper.GetNewFile(origFile, newFile);

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    Padding paddingBlock = null;
                    foreach (var block in flac.Metadata)
                    {
                        if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Padding)
                        {
                            paddingBlock = (Padding)block;
                        }
                    }

                    paddingBlock.EmptyBitCount = newPaddingSize; // Set empty bytes to 2

                    // Save flac file
                    flac.Save();
                }
                using (FlacFile flac = new FlacFile(newFile))
                {
                    Padding paddingBlock = null;
                    foreach (var block in flac.Metadata)
                    {
                        if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Padding)
                        {
                            paddingBlock = (Padding)block;
                        }
                    }

                    Assert.IsNotNull(paddingBlock);

                    Assert.AreEqual<UInt32>(newPaddingSize, paddingBlock.EmptyBitCount);
                }
            }
            finally
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
            }
        }

        [TestMethod, TestCategory("Write Tests")]
        public void CopyOpenEditAndSaveVorbisComments()
        {
            string origFile = @"Data\testfile1.flac";
            string newFile = @"Data\testfile1_temp.flac";
            // Tests if we can load up a flac file, update the artist and title in the vorbis comments
            // save the file and then reload the file and see the changes.
            FileHelper.GetNewFile(origFile, newFile);
            
            string newArtist = String.Empty;
            string newTitle = String.Empty;

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    Assert.IsNotNull(flac.VorbisComment);
                    string artist = flac.VorbisComment["ARTIST"];
                    string title = flac.VorbisComment.Title;
                    newArtist = String.Format("{0}_Edited", artist);
                    newTitle = String.Format("{0}_Edited", title);
                    flac.VorbisComment["ARTIST"] = newArtist;
                    flac.VorbisComment.Title = newTitle;

                    // Save flac file
                    flac.Save();
                }
                using (FlacFile flac = new FlacFile(newFile))
                {
                    Assert.IsNotNull(flac.VorbisComment);
                    Assert.AreEqual(flac.VorbisComment.Title, newTitle);
                    Assert.AreEqual(flac.VorbisComment.Artist, newArtist);
                }
            }
            finally
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
            }
        }

        [TestMethod, ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpMaxTracksExceededException)), TestCategory("Write Tests")]
        public void OverflowCueSheetTracks()
        {
            string flacFile = @"Data\testfile4.flac";

            using (FlacFile file = new FlacFile(flacFile))
            {
                file.CueSheet = new CueSheet();

                for (int i = 0; i < 100; i++)
                {
                    file.CueSheet.Tracks.Add(new CueSheetTrack());
                }

                // This guy should throw an exception ...
                file.CueSheet.Tracks.Add(new CueSheetTrack());
            }
        }

        [TestMethod, ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpMaxTrackIndicesExceededException)), TestCategory("Write Tests")]
        public void OverflowCueSheetTrackIndexPoints()
        {
            string flacFile = @"Data\testfile4.flac";

            using (FlacFile file = new FlacFile(flacFile))
            {
                file.CueSheet = new CueSheet();

                file.CueSheet.Tracks.Add(new CueSheetTrack());

                for (int i = 0; i < 100; i++)
                {
                    file.CueSheet.Tracks[0].IndexPoints.Add(new CueSheetTrackIndex());
                }

                // This guy should throw an exception ...
                file.CueSheet.Tracks[0].IndexPoints.Add(new CueSheetTrackIndex());
            }
        }

        [TestMethod, TestCategory("Write Tests")]
        public void CopyOpenEditAndSaveCueSheet()
        {
            string newMediaCatalog = "test";
            Boolean newIsCDCueSheet = false;
            ulong newLeadInSampleCount = 100;

            string origFile = @"Data\testfile4.flac";
            string newFile = @"Data\testfile4_temp.flac";

            FileHelper.GetNewFile(origFile, newFile);

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    var cueSheet = flac.CueSheet;
                    Assert.IsNotNull(cueSheet);

                    cueSheet.MediaCatalog = newMediaCatalog;
                    cueSheet.IsCDCueSheet = newIsCDCueSheet;
                    cueSheet.LeadInSampleCount = newLeadInSampleCount;

                    flac.Save();
                }
                using (FlacFile flac = new FlacFile(newFile))
                {
                    Assert.IsNotNull(flac.CueSheet);
                    Assert.AreEqual(newMediaCatalog, flac.CueSheet.MediaCatalog);
                    Assert.AreEqual(newIsCDCueSheet, flac.CueSheet.IsCDCueSheet);
                    Assert.AreEqual(newLeadInSampleCount, flac.CueSheet.LeadInSampleCount);
                }
            }
            finally
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
            }
        }

        [TestMethod(), ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException)), TestCategory("Write Tests")]
        public void SaveCueSheetWithoutCorrectLeadOutTrack()
        {
            string origFile = @"Data\testfile4.flac";
            string newFile = @"Data\testfile4_temp.flac";

            FileHelper.GetNewFile(origFile, newFile);

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    var cueSheet = flac.CueSheet;
                    Assert.IsNotNull(cueSheet);

                    CueSheetTrack newTrack = new CueSheetTrack();
                    newTrack.IsAudioTrack = true;
                    newTrack.IsPreEmphasis = false;
                    newTrack.TrackNumber = (byte)(55); // a non-lead-out track

                    flac.CueSheet.Tracks.Add(newTrack); // Add the track as last track ...

                    // The save should not allow this.
                    flac.Save();
                }
            }
            finally
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
            }
        }

        [TestMethod(), ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException)), TestCategory("Write Tests")]
        public void SaveCueSheetWithoutTracks()
        {
            string origFile = @"Data\testfile4.flac";
            string newFile = @"Data\testfile4_temp.flac";

            FileHelper.GetNewFile(origFile, newFile);

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    var cueSheet = flac.CueSheet;
                    Assert.IsNotNull(cueSheet);

                    cueSheet.Tracks.Clear();

                    // The save should not allow this since we must have at least a lead-out track.
                    flac.Save();
                }
            }
            finally
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
            }
        }

        [TestMethod(), TestCategory("Write Tests")]
        public void CopyOpenAddAndSaveCueSheetTracks()
        {
            string origFile = @"Data\testfile4.flac";
            string newFile = @"Data\testfile4_temp.flac";

            byte oldTrackCount = 0;
            ulong oldOffset = 0;
            ulong newOffset = 1000;
            string anISRC = "JMK401400212";

            byte firstIndexPointNr = 4;
            ulong firstIndexPointOffset = 356;
            byte secondIndexPointNr = 5;
            ulong secondIndexPointOffset = 1000;

            FileHelper.GetNewFile(origFile, newFile);

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    var cueSheet = flac.CueSheet;
                    Assert.IsNotNull(cueSheet);

                    oldTrackCount = cueSheet.TrackCount;

                    CueSheetTrack newTrack = new CueSheetTrack();
                    newTrack.IsAudioTrack = true;
                    newTrack.IsPreEmphasis = false;
                    newTrack.ISRC = anISRC;
                    newTrack.TrackNumber = (byte)(oldTrackCount + 1);
                    oldOffset = cueSheet.Tracks[cueSheet.Tracks.Count - 2].TrackOffset;
                    newOffset += oldOffset;
                    newTrack.TrackOffset = newOffset;

                    CueSheetTrackIndex indexPoint = new CueSheetTrackIndex();
                    indexPoint.IndexPointNumber = firstIndexPointNr;
                    indexPoint.Offset = firstIndexPointOffset;
                    newTrack.IndexPoints.Add(indexPoint);
                    indexPoint = new CueSheetTrackIndex();
                    indexPoint.IndexPointNumber = secondIndexPointNr;
                    indexPoint.Offset = secondIndexPointOffset;
                    newTrack.IndexPoints.Add(indexPoint);

                    // Insert the track just before the lead-out track ...
                    flac.CueSheet.Tracks.Insert(flac.CueSheet.Tracks.Count - 1, newTrack);

                    flac.Save();
                }
                using (FlacFile flac = new FlacFile(newFile))
                {
                    Assert.IsNotNull(flac.CueSheet);

                    // first verify that the last track is our track (ignoring the lead-out track ...)
                    var lastTrack = flac.CueSheet.Tracks[flac.CueSheet.TrackCount - 2];

                    Assert.AreEqual<bool>(true, lastTrack.IsAudioTrack);
                    Assert.AreEqual<bool>(false, lastTrack.IsPreEmphasis);
                    Assert.AreEqual<string>(anISRC, lastTrack.ISRC);
                    Assert.AreEqual<byte>(flac.CueSheet.TrackCount, lastTrack.TrackNumber);
                    Assert.AreEqual<ulong>(newOffset, lastTrack.TrackOffset);

                    // Now check if our two index points are still there as well
                    Assert.AreEqual<byte>(2, lastTrack.IndexPointCount);
                    Assert.AreEqual<byte>(firstIndexPointNr, lastTrack.IndexPoints[0].IndexPointNumber);
                    Assert.AreEqual<ulong>(firstIndexPointOffset, lastTrack.IndexPoints[0].Offset);
                    Assert.AreEqual<byte>(secondIndexPointNr, lastTrack.IndexPoints[1].IndexPointNumber);
                    Assert.AreEqual<ulong>(secondIndexPointOffset, lastTrack.IndexPoints[1].Offset);
                }
            }
            finally
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
            }
        }

    }
}
