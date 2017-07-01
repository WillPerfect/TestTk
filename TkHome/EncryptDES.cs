using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace TkHome
{
    class EncryptDES
    {
        private static byte[] _btIV = { 0x14, 0xB8, 0x3A, 0xA3, 0xEE, 0x4D, 0xC3, 0xDE };  //默认向量
        private static byte[] _btKey = { 0x69, 0x9C, 0x03, 0xBD, 0x2E, 0x98, 0x69, 0x73, 0x3C, 0xAD, 0xB7, 0xC9, 0x5B, 0x40, 0xB1, 0xE9 };  //默认秘钥
        private static string Encrypt(string sourceString, byte[] btKey, byte[] btIV)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] inData = Encoding.UTF8.GetBytes(sourceString);
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);

                            cs.FlushFinalBlock();
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                    catch
                    {
                        return sourceString;
                    }
                }
            }
            catch { }

            return "DES加密出错";
        }

        private static string Decrypt(string encryptedString, byte[] btKey, byte[] btIV)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);

                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch
                {
                    return encryptedString;
                }
            }
        }

        // 计算秘钥
        private static void GetKeyAndIV(out byte[] key, out byte[] iv)
        {
            byte[] userNameMd5 = EncryptMd5.Encrypt(Form1.UserName);
            key = new byte[userNameMd5.Length / 2]; // 最终秘钥
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = (byte)(_btKey[i] ^ userNameMd5[userNameMd5.Length - i - 1]);
            }

            iv = new byte[userNameMd5.Length / 2]; // 最终IV
            for (int i = 0; i < iv.Length; i++)
            {
                iv[i] = (byte)(_btIV[i] ^ userNameMd5[i]);
            }
        }

        public static string Encrypt(string sourceString)
        {
            byte[] key, iv;
            GetKeyAndIV(out key, out iv);
            return Encrypt(sourceString, key, iv);
        }

        public static string Decrypt(string encryptedString)
        {
            byte[] key, iv;
            GetKeyAndIV(out key, out iv);
            return Decrypt(encryptedString, key, iv);
        }
    }
}
