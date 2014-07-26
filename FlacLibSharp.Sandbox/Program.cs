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
            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {
                // Access to the StreamInfo class (actually this should ALWAYS be there ...)
                var streamInfo = file.StreamInfo;
                if (streamInfo != null)
                    Console.WriteLine("Flac audio length in seconds: {0}", file.StreamInfo.Duration);

                // Access to the VorbisComment IF it exists in the file
                var vorbisComment = file.VorbisComment;
                if (vorbisComment != null)
                    Console.WriteLine("Artist - Title: {0} - {1}", vorbisComment.Artist, vorbisComment.Title);

                // Get all other types of metdata blocks:
                var metadata = file.Metadata;
                foreach (MetadataBlock block in metadata)
                    Console.WriteLine("{0} metadata block.", block.Header.Type);

            }

            Console.ReadLine();
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
                    string artist = flac.VorbisComment["ARTIST"];
                    string title = flac.VorbisComment.Title;
                    newArtist = String.Format("{0}_Edited", artist);
                    newTitle = String.Format("{0}_Edited", title);
                    flac.VorbisComment["ARTIST"] = newArtist;
                    flac.VorbisComment.Title = newTitle;

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
