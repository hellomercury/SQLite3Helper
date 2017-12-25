using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SQLite3Helper
{
    public class SQLite3Utility
    {
        private static MD5 md5;
        public static string GetFileMD5(string InFilePath)
        {
            try
            {
                if (null == md5)
                {
                    md5 = new MD5CryptoServiceProvider();
                }

                if (File.Exists(InFilePath))
                {
                    byte[] data;

                    using (FileStream stream = new FileStream(InFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        int len = (int)stream.Length;
                        data = new byte[len];
                        stream.Read(data, 0, len);
                        stream.Close();
                    }

                    byte[] result = md5.ComputeHash(data);
                    StringBuilder sb = new StringBuilder(32);
                    for (int i = 0; i < result.Length; i++)
                    {
                        sb.Append(result[i].ToString("x2"));
                    }

                    return sb.ToString();
                }

                return "";
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return "";
            }
        }
    }
}

