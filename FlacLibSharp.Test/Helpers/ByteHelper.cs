using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlacLibSharp.Test.Helpers
{
    class ByteHelper
    {

        public static string ByteArrayToString(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte d in data)
                hex.AppendFormat("{0:x2}", d);
            return hex.ToString();
        }

    }
}
