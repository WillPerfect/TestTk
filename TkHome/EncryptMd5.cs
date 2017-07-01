using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TkHome
{
    class EncryptMd5
    {
        public static byte[] Encrypt(string text)
        {
            byte[] result = Encoding.Default.GetBytes(text);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return output;
        }
    }
}
