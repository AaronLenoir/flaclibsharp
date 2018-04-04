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
            // Tests if we can load up a flac file, update the artist and title in the vorbis comments
            // save the file and then reload the file and see the changes.
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
    }
}
