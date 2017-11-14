using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FlacLibSharp.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            DuplicateMetadata();

            // Access to the StreamInfo class
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                Console.WriteLine("Flac audio length in seconds: {0}", file.StreamInfo.Duration);
            }

            // Access to the VorbisComment IF it exists in the file
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                var vorbisComment = file.VorbisComment;
                if (vorbisComment != null)
                {
                    Console.WriteLine("Artist - Title: {0} - {1}", vorbisComment.Artist, vorbisComment.Title);
                }
            }

            // Access to the VorbisComment with multiple values for a single field
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                var vorbisComment = file.VorbisComment;
                if (vorbisComment != null)
                {
                    foreach (var value in vorbisComment.Artist)
                    {
                        Console.WriteLine("Artist: {0}", value);
                    }
                }
            }

            // Iterate through all VorbisComment tags
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                var vorbisComment = file.VorbisComment;
                if (vorbisComment != null)
                {
                    foreach (var tag in vorbisComment)
                    {
                        Console.WriteLine("{0}: {1}", tag.Key, tag.Value);
                    }
                }
            }

            // Get all other types of metdata blocks
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                var metadata = file.Metadata;
                foreach (MetadataBlock block in metadata) { 
                    Console.WriteLine("{0} metadata block.", block.Header.Type);
                }
            }

            Console.ReadLine();
        }

        public static void DuplicateMetadata()
        {
            File.Copy(@"Data\testfile4.flac", @"Data\testfile4_tmp.flac", true);
            using (FlacFile file = new FlacFile(@"Data\testfile4_tmp.flac"))
            {
                /* Appinfo is fine! */
                /*
                var appInfo = file.ApplicationInfo;
                if (appInfo == null)
                {
                    appInfo = new ApplicationInfo();
                    appInfo.ApplicationID = 10;
                    appInfo.ApplicationData = new byte[] { 10, 20, 30 };
                    file.Metadata.Add(appInfo);
                    file.Metadata.Add(appInfo);
                }
                */
                
                /* Cuesheet is fine ?*/
                /*
                var cueSheet = file.CueSheet;
                file.Metadata.Add(cueSheet);
                */

                /* Vorbis also fine! */
                /*
                var vorbis = file.VorbisComment;
                if (vorbis == null)
                {
                    vorbis = new VorbisComment();

                    vorbis.Album = "My Test Album";

                    file.Metadata.Add(vorbis);

                    file.Metadata.Add(vorbis);
                }
                
                file.Metadata.Add(vorbis);
                */
                
                /* Metadata not fine! */
                /*
                var streaminfo = file.StreamInfo;
                file.Metadata.Add(streaminfo);
                */

                /* SeekTable is fine */
                /*
                var seekTable = new SeekTable();
                seekTable.SeekPoints.Add(new SeekPoint() { ByteOffset = 0, FirstSampleNumber = 0, IsPlaceHolder = false, NumberOfSamples = 100 });
                file.Metadata.Add(seekTable);
                file.Metadata.Add(seekTable);
                */

                /* We know picture is fine */

                /* Padding is fine */

                /* So everything fine, except StreamInfo */

                file.Save();
            }
        }

        public static void CopyOpenAndSaveStreamInfo()
        {
            string origFile = @"Data\testfile1.flac";
            string newFile = @"Data\testfile1_temp.flac";
            if (File.Exists(newFile))
            {
                File.Delete(newFile);
            }
            File.Copy(origFile, newFile);

            string newArtist = String.Empty;
            string newTitle = String.Empty;

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    foreach (var block in flac.Metadata)
                    {
                        if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Padding)
                        {
                            Console.WriteLine("Before Modif: Padding length in bit: {0}", ((Padding)block).EmptyBitCount);
                            ((Padding)block).EmptyBitCount = 8; // Remove some padding ...
                        }
                    }

                    foreach (var block in flac.Metadata)
                    {
                        if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Padding)
                        {
                            Console.WriteLine("After Modif: Padding length in bit: {0}", ((Padding)block).EmptyBitCount);
                        }
                    }

                    // Save flac file
                    flac.Save();
                }
                using (FlacFile flac = new FlacFile(newFile))
                {
                    // This will check whether the save did correctly write back the streaminfo
                    var info = flac.StreamInfo;
                    string md5sum = ByteArrayToString(info.MD5Signature);

                    foreach (var block in flac.Metadata)
                    {
                        if (block.Header.Type == MetadataBlockHeader.MetadataBlockType.Padding)
                        {
                            Console.WriteLine("After Save: Padding length in bit: {0}", ((Padding)block).EmptyBitCount);
                        }
                    }

                    //Assert.AreEqual(md5sum, "1d2e54a059ea776787ef66f1f93d3e34");
                    //Assert.AreEqual(info.MinimumBlockSize, 4096);
                    //Assert.AreEqual(info.MaximumBlockSize, 4096);
                    //Assert.AreEqual(info.MinimumFrameSize, (uint)1427);
                    //Assert.AreEqual(info.MaximumFrameSize, (uint)7211);
                    //Assert.AreEqual(info.SampleRateHz, (uint)44100);
                    //Assert.AreEqual(info.Channels, 1);
                    //Assert.AreEqual(info.BitsPerSample, 16);
                    //Assert.AreEqual(info.Samples, 1703592);
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

        public static void CopyOpenEditAndSaveVorbisComments()
        {
            string origFile = @"Data\testfile1.flac";
            string newFile = @"Data\testfile1_temp.flac";
            // Tests if we can load up a flac file, update the artist and title in the vorbis comments
            // save the file and then reload the file and see the changes.
            if (File.Exists(newFile))
            {
                File.Delete(newFile);
            }
            File.Copy(origFile, newFile);

            string newArtist = String.Empty;
            string newTitle = String.Empty;

            try
            {
                using (FlacFile flac = new FlacFile(newFile))
                {
                    string artist = flac.VorbisComment["ARTIST"].Value;
                    string title = flac.VorbisComment.Title.Value;
                    newArtist = String.Format("{0}_Edited", artist);
                    newTitle = String.Format("{0}_Edited", title);
                    flac.VorbisComment["ARTIST"].Value = newArtist;
                    flac.VorbisComment.Title.Value = newTitle;

                    // Save flac file
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

        public static string ByteArrayToString(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte d in data)
                hex.AppendFormat("{0:x2}", d);
            return hex.ToString();
        }

    }
}
