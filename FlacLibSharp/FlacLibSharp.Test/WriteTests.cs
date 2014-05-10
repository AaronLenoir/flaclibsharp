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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod, ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpMaxTracksExceededException))]
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

        [TestMethod]
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

    }
}
