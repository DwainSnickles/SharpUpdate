using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SharpUpdate
{
    internal enum HashType
    {
        MD5,
        SHA1,
        SHA512
    }

    internal static class Hasher
    {
        static String FilePath;

        internal static string HashFile(string filePath, HashType algo)
        {
            FilePath = filePath;
            Properties.Settings.Default.MD5CheckSum = CheckMD5(FilePath);
            Properties.Settings.Default.Save();

            switch (algo)
            {
                case SharpUpdate.HashType.MD5:
                    return MakeHashString(MD5.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));

                case SharpUpdate.HashType.SHA1:
                    return MakeHashString(SHA1.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));

                case SharpUpdate.HashType.SHA512:
                    return MakeHashString(SHA512.Create().ComputeHash(new FileStream(filePath, FileMode.Open)));

                default:
                    return "";
            }
        }

        private static string MakeHashString(byte[] hash)
        {
            StringBuilder s = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
                s.Append(b.ToString("X2").ToLower());

            return s.ToString();
        }

        public static string CheckMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(FilePath))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }
    }
}
