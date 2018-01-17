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

        public static string ConvertToString(SQLite3Constraint InConstraint)
        {
            string result = string.Empty;
            if ((InConstraint & SQLite3Constraint.PrimaryKey) != 0)
                result += " PrimaryKey ";
            if ((InConstraint & SQLite3Constraint.Unique) != 0)
                result += " Unique ";
            if ((InConstraint & SQLite3Constraint.AutoIncrement) != 0)
                result += " AutoIncrement ";
            if ((InConstraint & SQLite3Constraint.NotNull) != 0)
                result += " NotNull ";

            return result == string.Empty ? string.Empty : result.Remove(result.Length - 1, 1);
        }
    }
}

