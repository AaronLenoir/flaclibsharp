using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlacLibSharp.Test.Helpers
{
    public class FileHelper
    {

        public static void GetNewFile(string origFile, string newFile)
        {
            if (File.Exists(newFile))
            {
                File.Delete(newFile);
            }
            File.Copy(origFile, newFile);
        }

    }
}
