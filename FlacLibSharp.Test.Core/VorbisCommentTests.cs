using FlacLibSharp.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace FlacLibSharp.Test
{
    [TestClass]
    public class VorbisCommentTests
    {
        [TestMethod, TestCategory("VorbisCommentTests")]
        public void IterateThroughVorbisCommentsShouldSeeAllVorbisComments()
        {
            using (FlacFile file = new FlacFile(Path.Combine("Data", "testfile1.flac")))
            {
                var counter = 0;
                foreach (var vorbisComment in file.VorbisComment)
                {
                    counter++;
                }

                Assert.AreEqual(9, counter);
            }
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void IterateThroughVorbisCommentsShouldSeeAllVorbisCommentsWithKey()
        {
            var expectedTags = new List<string>
            {
                "PUBLISHER", "COMPOSER", "COMMENT", "ARTIST", "TITLE", "ALBUM", "DATE", "TRACKNUMBER", "GENRE"
            };
            using (FlacFile file = new FlacFile(Path.Combine("Data", "testfile1.flac")))
            {
                foreach (var vorbisComment in file.VorbisComment)
                {
                    if (expectedTags.Contains(vorbisComment.Key))
                    {
                        expectedTags.Remove(vorbisComment.Key);
                    }
                    else
                    {
                        Assert.Fail($"Found unexpected vorbis comment, key = {vorbisComment.Key}");
                    }
                }

                Assert.AreEqual(0, expectedTags.Count);
            }
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void IterateThroughVorbisCommentsShouldSeeAllVorbisCommentsWithCorrectValues()
        {
            using (FlacFile file = new FlacFile(Path.Combine("Data", "testfile1.flac")))
            {
                foreach (var vorbisComment in file.VorbisComment)
                {
                    if (vorbisComment.Key == "ARTIST")
                    {
                        Assert.AreEqual("Ziggystar", vorbisComment.Value.Value, "Artist did not have the expected value.");
                    }
                    if (vorbisComment.Key == "TITLE")
                    {
                        Assert.AreEqual("Roland jx3p demo", vorbisComment.Value.Value, "Title did not have the expected value.");
                    }
                }
            }
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void WritingTwoArtistsShouldResultInTwoArtistsRead()
        {
            string origFile = Path.Combine("Data", "testfile5.flac");
            string newFile = Path.Combine("Data", "testfile5_temp.flac");
            FileHelper.GetNewFile(origFile, newFile);

            using (FlacFile file = new FlacFile(Path.Combine("Data", "testfile5_temp.flac")))
            {
                var vorbisComment = new VorbisComment();

                vorbisComment["ARTIST"] = new VorbisCommentValues(new string[] { "Artist A", "Artist B" });

                file.Metadata.Add(vorbisComment);

                file.Save();
            }

            using (FlacFile file = new FlacFile(Path.Combine("Data", "testfile5_temp.flac")))
            {
                Assert.IsNotNull(file.VorbisComment);
                var artistValues = file.VorbisComment["ARTIST"];
                Assert.AreEqual(2, artistValues.Count);
                Assert.AreEqual("Artist A", artistValues[0]);
                Assert.AreEqual("Artist B", artistValues[1]);
            }
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void CueSheetVorbisCommentShouldBeInProperty()
        {
            var cueSheetPath = Path.Combine("Data", "cuesheet.txt");
            var cueSheetData = File.ReadAllText(cueSheetPath);

            string origFile = Path.Combine("Data", "testfile5.flac");
            string newFile = Path.Combine("Data", "testfile5_temp.flac");
            FileHelper.GetNewFile(origFile, newFile);

            using (FlacFile file = new FlacFile(Path.Combine("Data", "testfile5_temp.flac")))
            {
                var vorbisComment = new VorbisComment();

                vorbisComment["CUESHEET"] = new VorbisCommentValues(cueSheetData);

                file.Metadata.Add(vorbisComment);

                file.Save();
            }

            using (FlacFile file = new FlacFile(Path.Combine("Data", "testfile5_temp.flac")))
            {
                var cueSheetDataFromFile = file.VorbisComment.CueSheet;
                Assert.AreEqual(cueSheetData, cueSheetDataFromFile.Value);
            }
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void RemoveMethodShouldRemoveTag()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment["ARTIST"] = new VorbisCommentValues("Aaron");
            vorbisComment.Remove("ARTIST");
            Assert.IsTrue(vorbisComment.Artist.Count == 0, "Tag was not removed.");
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void RemoveMethodShouldNotThrowErrorIfTagNotFound()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment.Remove("BLABLABLA");
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void RemoveMethodShouldRemoveTagWithSpecificValue()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment["ARTIST"] = new VorbisCommentValues("Aaron");
            vorbisComment.Remove("ARTIST", "Lenoir");
            Assert.IsTrue(vorbisComment.Artist.Count == 1, "Tag was removed when it shouldn't have been.");
            vorbisComment.Remove("ARTIST", "Aaron");
            Assert.IsTrue(vorbisComment.Artist.Count == 0, "Tag was not removed.");
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void AddMethodShouldAddSingleValueTag()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment.Add("ARTIST", "Aaron");
            Assert.AreEqual(1, vorbisComment.Artist.Count);
            Assert.AreEqual("Aaron", vorbisComment.Artist.Value);
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void AddMethodShouldAppendNewValue()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment.Add("ARTIST", "Aaron");
            vorbisComment.Add("ARTIST", "Lenoir");
            Assert.AreEqual(2, vorbisComment.Artist.Count);
            Assert.AreEqual("Aaron", vorbisComment.Artist.Value);
            Assert.AreEqual("Aaron", vorbisComment.Artist[0]);
            Assert.AreEqual("Lenoir", vorbisComment.Artist[1]);
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void AddMethodShouldAddMultipleValues()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment.Add("ARTIST", new string[] { "Aaron", "Lenoir" });
            Assert.AreEqual(2, vorbisComment.Artist.Count);
            Assert.AreEqual("Aaron", vorbisComment.Artist[0]);
            Assert.AreEqual("Lenoir", vorbisComment.Artist[1]);
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void AddMethodShouldAppendMultipleValues()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment.Add("ARTIST", new string[] { "Aaron", "Lenoir" });
            vorbisComment.Add("ARTIST", "Third");
            Assert.AreEqual(3, vorbisComment.Artist.Count);
            Assert.AreEqual("Aaron", vorbisComment.Artist[0]);
            Assert.AreEqual("Lenoir", vorbisComment.Artist[1]);
            Assert.AreEqual("Third", vorbisComment.Artist[2]);
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void ReplaceMethodShouldReplaceAllValues()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment.Add("ARTIST", new string[] { "Aaron", "Lenoir" });
            vorbisComment.Replace("ARTIST", "Test");
            Assert.AreEqual(1, vorbisComment.Artist.Count);
            Assert.AreEqual("Test", vorbisComment.Artist[0]);
        }

        [TestMethod, TestCategory("VorbisCommentTests")]
        public void ReplaceMethodMultipleValuesShouldReplaceAllValues()
        {
            var vorbisComment = new VorbisComment();
            vorbisComment.Add("ARTIST", "Aaron");
            vorbisComment.Replace("ARTIST", new string[] { "Test", "Lenoir" });
            Assert.AreEqual(2, vorbisComment.Artist.Count);
            Assert.AreEqual("Test", vorbisComment.Artist[0]);
            Assert.AreEqual("Lenoir", vorbisComment.Artist[1]);
        }
    }
}
