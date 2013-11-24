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
            Console.WriteLine("Get Album: " + FastFlac.GetAlbum(@"Data\testfile1.flac"));
            Console.WriteLine("Get Title: " + FastFlac.GetTitle(@"Data\testfile1.flac"));
            Console.WriteLine("Get Artist: " + FastFlac.GetArtist(@"Data\testfile1.flac"));
            Console.WriteLine("Get Duration: " + FastFlac.GetDuration(@"Data\testfile1.flac"));

            var data = FastFlac.GetMetaData("file.flac");
            FlacLibSharp.Metadata.StreamInfo = FastFlac.GetStreamInfo("file.flac");

            using (FlacFile flac = new FlacFile(@"file1.flac"))
            {
                var data = flac.Metadata;
                foreach (var block in data)
                {
                    Console.WriteLine("Metadata block found of type: {0}", block.Header.Type);
                }
            }

            Console.ReadLine();
        }


    }
}
