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
    
        /// <summary>
        /// Will create a vorbis comment block of metadata, save the file and re-open it.
        /// </summary>
        [TestMethod]
        public void CreateVorbisComment()
        {
            string artist = "Some Artist";
            string albumName = "Test Album";
            string customTag = "PROJECT";
            string customTagValue = "FlacLibSharp";
            string customTag2 = "Author";
            string customTag2Value = "Aaron";
            string title = "Test Track";
            string title2 = "Title modified";
            string titleTag = "Title";

            FileHelper.GetNewFile(origFile, newFile);

            using (FlacFile flac = new FlacFile(newFile))
            {
                VorbisComment vorbisComment = new VorbisComment();

                vorbisComment.Album = albumName;
                vorbisComment.Artist = artist;
                vorbisComment[customTag] = customTagValue;
                vorbisComment[customTag2] = customTag2Value;
                vorbisComment.Title = title;
                vorbisComment[titleTag] = title2;

                flac.Metadata.Add(vorbisComment);

                flac.Save();
            }

            using (FlacFile flac = new FlacFile(newFile))
            {
                VorbisComment vorbisComment = flac.VorbisComment;

                Assert.AreEqual<string>(albumName, vorbisComment.Album);
                Assert.AreEqual<string>(artist, vorbisComment.Artist);
                Assert.AreEqual<string>(customTagValue, vorbisComment[customTag]);
                Assert.AreEqual<string>(customTag2Value, vorbisComment[customTag2.ToUpper()]);
                Assert.AreEqual<string>(title2, vorbisComment.Title);
                Assert.AreEqual<string>(title2, vorbisComment[titleTag]);
            }
        }

        /// <summary>
        /// Will create a cuesheet with no tracks, should raise an exception on Save.
        /// </summary>
        [TestMethod, ExpectedException(typeof(FlacLibSharp.Exceptions.FlacLibSharpInvalidFormatException))]
        public void CreateInvalidCueSheet()
        {
            FileHelper.GetNewFile(origFile, newFile);

            using (FlacFile flac = new FlacFile(newFile))
            {
                CueSheet sheet = new CueSheet();

                flac.Metadata.Add(sheet);

                flac.Save();
            }
        }

        [TestMethod]
        public void CreateValidCueSheet()
        {
            string anISRC = "JMK401400212";

            byte firstIndexPointNr = 4;
            ulong firstIndexPointOffset = 356;
            byte secondIndexPointNr = 5;
            ulong secondIndexPointOffset = 1000;

            FileHelper.GetNewFile(origFile, newFile);

            using (FlacFile flac = new FlacFile(newFile))
            {
                CueSheet cueSheet = new CueSheet();

                CueSheetTrack newTrack = new CueSheetTrack();
                newTrack.IsAudioTrack = true;
                newTrack.IsPreEmphasis = false;
                newTrack.ISRC = anISRC;
                newTrack.TrackNumber = 1;
                newTrack.TrackOffset = 0;

                CueSheetTrackIndex indexPoint = new CueSheetTrackIndex();
                indexPoint.IndexPointNumber = firstIndexPointNr;
                indexPoint.Offset = firstIndexPointOffset;
                newTrack.IndexPoints.Add(indexPoint);
                indexPoint = new CueSheetTrackIndex();
                indexPoint.IndexPointNumber = secondIndexPointNr;
                indexPoint.Offset = secondIndexPointOffset;
                newTrack.IndexPoints.Add(indexPoint);

                cueSheet.Tracks.Add(newTrack);

                // Create the lead-out track

                CueSheetTrack leadOut = new CueSheetTrack();
                leadOut.IsAudioTrack = false;
                leadOut.TrackNumber = CueSheet.CUESHEET_LEADOUT_TRACK_NUMBER_CDDA;
                cueSheet.Tracks.Add(leadOut);

                flac.Metadata.Add(cueSheet);

                flac.Save();
            }

            using (FlacFile flac = new FlacFile(newFile))
            {
                CueSheet cueSheet = flac.CueSheet;
                Assert.IsNotNull(cueSheet);

                Assert.AreEqual<byte>(2, cueSheet.TrackCount);

                CueSheetTrack track = cueSheet.Tracks[0];

                Assert.AreEqual<bool>(true, track.IsAudioTrack);
                Assert.AreEqual<bool>(false, track.IsPreEmphasis);
                Assert.AreEqual<string>(anISRC, track.ISRC);
                Assert.AreEqual<byte>(1, track.TrackNumber);
                Assert.AreEqual<ulong>(0, track.TrackOffset);

                Assert.AreEqual<byte>(2, track.IndexPointCount);
                Assert.AreEqual<byte>(firstIndexPointNr, track.IndexPoints[0].IndexPointNumber);
                Assert.AreEqual<ulong>(firstIndexPointOffset, track.IndexPoints[0].Offset);
                Assert.AreEqual<byte>(secondIndexPointNr, track.IndexPoints[1].IndexPointNumber);
                Assert.AreEqual<ulong>(secondIndexPointOffset, track.IndexPoints[1].Offset);
            }

        }

    }
}
