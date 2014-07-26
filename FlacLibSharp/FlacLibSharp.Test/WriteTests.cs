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
                CueSheet sheet = new CueSheet();

                for (int i = 0; i < 100; i++)
                {
                    sheet.Tracks.Add(new CueSheetTrack());
                }

                // This guy should throw an exception ...
                sheet.Tracks.Add(new CueSheetTrack());
            }
        }

        [TestMethod, ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpMaxTrackIndicesExceededException)), TestCategory("Write Tests")]
        public void OverflowCueSheetTrackIndexPoints()
        {
            string flacFile = @"Data\testfile4.flac";

            using (FlacFile file = new FlacFile(flacFile))
            {
                CueSheet sheet = new CueSheet();

                sheet.Tracks.Add(new CueSheetTrack());

                for (int i = 0; i < 100; i++)
                {
                    sheet.Tracks[0].IndexPoints.Add(new CueSheetTrackIndex());
                }

                // This guy should throw an exception ...
                sheet.Tracks[0].IndexPoints.Add(new CueSheetTrackIndex());
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


        [TestMethod(), TestCategory("Write Tests")]
        public void CopyOpenAddAndSavePicture()
        {
            string origFile = @"Data\testfile2.flac";
            string newFile = @"Data\testfile2_temp.flac";
            byte[] imageData = File.ReadAllBytes(@"Data\testimage.png");

            FileHelper.GetNewFile(origFile, newFile);

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    Picture pict = null;
                    
                    pict = new FlacLibSharp.Picture();
                    pict.ColorDepth = 24;
                    pict.Data = imageData;
                    pict.Description = "Small picture test ...";
                    pict.Height = 420;
                    pict.Width = 410;
                    pict.MIMEType = "image/png";
                    pict.PictureType = PictureType.ArtistLogotype;

                    flac.Metadata.Add(pict);

                    pict = new FlacLibSharp.Picture();
                    pict.ColorDepth = 24;
                    pict.Description = "Small URL picture test ...";
                    pict.Height = 768;
                    pict.Width = 1024;
                    pict.MIMEType = "-->";
                    pict.PictureType = PictureType.BrightColouredFish;
                    pict.URL = "http://38.media.tumblr.com/0e954b0469c281a9a09eb1378daada3e/tumblr_mh0cpm19zR1s3yrubo1_1280.jpg";

                    flac.Metadata.Add(pict);

                    flac.Save();
                }
                using (FlacFile flac = new FlacFile(newFile))
                {
                    List<Picture> pictures = flac.GetPictures();
                    Assert.IsTrue(pictures.Count > 0);

                    bool foundOurImage = false;
                    bool foundOurURL = false;
                    foreach (var pict in pictures)
                    {
                        if (pict.Description == "Small picture test ...")
                        {
                            Assert.AreEqual<uint>(24, pict.ColorDepth);
                            Assert.AreEqual<string>("Small picture test ...", pict.Description);
                            Assert.AreEqual<uint>(420, pict.Height);
                            Assert.AreEqual<uint>(410, pict.Width);
                            Assert.AreEqual<string>("image/png", pict.MIMEType);
                            Assert.AreEqual<PictureType>(PictureType.ArtistLogotype, pict.PictureType);

                            Assert.IsNotNull(pict.Data.Length);
                            Assert.AreEqual<int>(imageData.Length, pict.Data.Length);
                            for (int i = 0; i < imageData.Length; i++)
                            {
                                Assert.AreEqual<byte>(imageData[i], pict.Data[i], "Written picture data does not match read picture data.");
                            }

                            foundOurImage = true;
                        }

                        if (pict.Description == "Small URL picture test ...")
                        {
                            Assert.AreEqual<uint>(24, pict.ColorDepth);
                            Assert.AreEqual<string>("Small URL picture test ...", pict.Description);
                            Assert.AreEqual<uint>(768, pict.Height);
                            Assert.AreEqual<uint>(1024, pict.Width);
                            Assert.AreEqual<string>("-->", pict.MIMEType);
                            Assert.AreEqual<PictureType>(PictureType.BrightColouredFish, pict.PictureType);
                            Assert.AreEqual<string>("http://38.media.tumblr.com/0e954b0469c281a9a09eb1378daada3e/tumblr_mh0cpm19zR1s3yrubo1_1280.jpg", pict.URL);
                            foundOurURL = true;
                        }
                    }

                    Assert.IsTrue(foundOurImage);
                    Assert.IsTrue(foundOurURL);
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
        public void CopyOpenEditAndSaveSeekTable()
        {
            ulong targetSampleNumber = 704512; // Will remove the seekpoint for this sample number ...
            ulong expectedNumberOfSamples; // Will be set while creating a new seekpoint
            ulong expectedByteOffset; // Will be set while creating a new seekpoint

            string origFile = @"Data\testfile3.flac";
            string newFile = @"Data\testfile3_temp.flac";

            FileHelper.GetNewFile(origFile, newFile);

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    Assert.IsNotNull(flac.SeekTable);

                    // Will remove the at the given sample number ...
                    SeekPoint oldSeekpoint = flac.SeekTable.SeekPoints[targetSampleNumber];
                    flac.SeekTable.SeekPoints.Remove(targetSampleNumber);

                    // Create a new Seekpoint, a little further
                    SeekPoint newSeekpoint = new SeekPoint();
                    newSeekpoint.FirstSampleNumber = targetSampleNumber + 1000; // Put it a 1000 samples further ...
                    newSeekpoint.NumberOfSamples = (ushort)(oldSeekpoint.NumberOfSamples - (ushort)1000); // Since we are a 1000 further, we contain a 1000 less samples
                    expectedNumberOfSamples = newSeekpoint.NumberOfSamples;
                    newSeekpoint.ByteOffset = oldSeekpoint.ByteOffset;
                    expectedByteOffset = newSeekpoint.ByteOffset;
                    flac.SeekTable.SeekPoints.Add(newSeekpoint);

                    // Create a placeholder seekpoint
                    SeekPoint placeHolder = new SeekPoint();
                    placeHolder.FirstSampleNumber = ulong.MaxValue;
                    // The other two values are "undefined"
                    Assert.IsTrue(placeHolder.IsPlaceHolder); // Already assert that the object itself handles this flac correctly.
                    flac.SeekTable.SeekPoints.Add(placeHolder);

                    // This should actually be allowed according to the FLAC format (multiple placeHolders are ok, but they must occur at the end of the seektable)
                    flac.SeekTable.SeekPoints.Add(placeHolder);

                    // Check if we actually get "2" placeholders ...
                    Assert.AreEqual<int>(2, flac.SeekTable.SeekPoints.Placeholders);

                    flac.Save();
                }
                using (FlacFile flac = new FlacFile(newFile))
                {
                    // Now we want to try and find our new SeekPoint
                    Assert.IsNotNull(flac.SeekTable);

                    Assert.IsFalse(flac.SeekTable.SeekPoints.ContainsKey(targetSampleNumber));
                    Assert.IsTrue(flac.SeekTable.SeekPoints.ContainsKey(targetSampleNumber + 1000));
                    SeekPoint newSeekpoint = flac.SeekTable.SeekPoints[targetSampleNumber + 1000];

                    Assert.AreEqual<ulong>(expectedNumberOfSamples, newSeekpoint.NumberOfSamples);
                    Assert.AreEqual<ulong>(expectedByteOffset, newSeekpoint.ByteOffset);

                    Assert.IsFalse(newSeekpoint.IsPlaceHolder);

                    // Check if we actually get "2" placeholders ...
                    Assert.AreEqual<int>(2, flac.SeekTable.SeekPoints.Placeholders);
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
