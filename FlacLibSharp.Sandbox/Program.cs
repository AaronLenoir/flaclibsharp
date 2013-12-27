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

            using (FlacFile file = new FlacFile(@"Data\testfile4.flac"))
            {
                var cueSheet = file.CueSheet;
            }

            Console.ReadLine();
        }


    }
}
