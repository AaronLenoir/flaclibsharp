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
        //    Console.WriteLine("Get Album: " + FastFlac.GetAlbum(@"Data\testfile1.flac"));
        //    Console.WriteLine("Get Title: " + FastFlac.GetTitle(@"Data\testfile1.flac"));
        //    Console.WriteLine("Get Artist: " + FastFlac.GetArtist(@"Data\testfile1.flac"));
        //    Console.WriteLine("Get Duration: " + FastFlac.GetDuration(@"Data\testfile1.flac"));


            using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
            {

            }

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


    }
}
