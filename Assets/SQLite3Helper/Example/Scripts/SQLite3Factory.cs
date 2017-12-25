using System;
using System.IO;
using UnityEngine;

namespace SQLite3Helper.Example
{
    public enum SQLite3OpenMode
    {
        ReadOnly,
        ReadWrite,
        OpenOrCreate,
        CreateNew,
        None
    }

    /// <summary>
    /// The Factory class of Factory mode. 
    /// </summary>
    public static class SQLite3Factory
    {
        /// <summary>
        /// Get SQLite3Operate instance by database name and open mode.
        /// </summary>
        /// <param name="InDbName">Name of database</param>
        /// <param name="InOpenMode">SQLite3 open mode.</param>
        /// <returns>The SQLite3 Operate instance.</returns>
        public static SQLite3Operate GetSQLite3Operate(string InDbName, SQLite3OpenMode InOpenMode)
        {
            switch (InOpenMode)
            {
                case SQLite3OpenMode.ReadOnly:
                    return ReadOnly(InDbName);

                case SQLite3OpenMode.ReadWrite:
                    return ReadWrite(InDbName);

                case SQLite3OpenMode.OpenOrCreate:
                    return OpenOrCreate(InDbName);

                case SQLite3OpenMode.CreateNew:
                    return OpenOrCreate(InDbName);
                    
                default:
                    throw new NotSupportedException("Can not support this mode.");
            }
        }

        /// <summary>
        /// Open a database to read the data, you can not make any changes to the database. 
        /// If database is not exist there will throw a FileLoadException.
        /// when first opened database program will copy it from streamingAssetsPath directory to the persistentDataPath directory,
        /// </summary>
        /// <param name="InDbName">Name of database.</param>
        /// <returns>The SQLite3Operate object.</returns>
        private static SQLite3Operate ReadOnly(string InDbName)
        {
            return Load(InDbName, SQLite3OpenFlags.ReadOnly);
        }

        /// <summary>
        /// Open an existing database for data read and write, 
        /// if the database does not exist will throw a FileLoadException. 
        /// If the database exists in the streamingAssetsPath directory, 
        /// the database will be copied to the persistentDataPath directory, 
        /// any operation on the database will not affect the database under the streamingAssetsPath directory.
        /// Note that the database may be modified by players, please check the correctness of the data before use.
        /// </summary>
        /// <param name="InDbName">Name of database.</param>
        /// <returns>The SQLite3Operate object.</returns>
        private static SQLite3Operate ReadWrite(string InDbName)
        {
            return Load(InDbName, SQLite3OpenFlags.ReadWrite);
        }

        /// <summary>
        /// Open or Create a database for data read and write, 
        /// if the database does not exist will create a new database on persistentDataPath directory. 
        /// Note that the database may be modified by players, please check the correctness of the data before use.
        /// </summary>
        /// <param name="InDbName">Name of database.</param>
        /// <returns>The SQLite3Operate object.</returns>
        private static SQLite3Operate OpenOrCreate(string InDbName)
        {
            string destinationPath = Path.Combine(Application.persistentDataPath, InDbName);
            return new SQLite3Operate(destinationPath, SQLite3OpenFlags.Create | SQLite3OpenFlags.ReadWrite);
        }

        /// <summary>
        /// Create a new database under the persistentDataPath directory for data read and write.
        /// </summary>
        /// <param name="InDbName">Name of database.</param>
        /// <returns>The SQLite3Operate object.</returns>
        private static SQLite3Operate CreateNew(string InDbName)
        {
            string destinationPath = Path.Combine(Application.persistentDataPath, InDbName);
            if (File.Exists(destinationPath)) File.Delete(destinationPath);
            return new SQLite3Operate(destinationPath, SQLite3OpenFlags.Create | SQLite3OpenFlags.ReadWrite);
        }

        /// <summary>
        /// Copy a exist database from the StreamingAssets path to PrersistentDataPath.
        /// And open it according the open flags.
        /// </summary>
        /// <returns>The SQLite3Operate object.</returns>
        /// <param name="InDbName">In db name.</param>
        /// <param name="InSQLite3OpenFlags">In SQLite3 open flags.</param>
        private static SQLite3Operate Load(string InDbName, SQLite3OpenFlags InSQLite3OpenFlags)
        {
            string destinationPath = Path.Combine(Application.persistentDataPath, InDbName);
            bool needCopy = true;

            if (File.Exists(destinationPath) && SQLite3OpenFlags.ReadOnly == InSQLite3OpenFlags)
            {
                if (PlayerPrefs.GetString("SQLITE_" + InDbName + "_MD5", string.Empty)
                    .Equals(SQLite3Utility.GetFileMD5(destinationPath)))
                    needCopy = false;
            }

            if (needCopy)
            {
#if UNITY_ANDROID
                string streamPath = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IOS
                string streamPath = Application.dataPath + "/Raw/";
#else
                string streamPath = Application.streamingAssetsPath + "/";
#endif

                string sourcePath = Path.Combine(streamPath, InDbName);

#if UNITY_ANDROID
                using(WWW www = new WWW(sourcePath))
                {
                    while (www.isDone){}
                    if (string.IsNullOrEmpty(www.error)) File.WriteAllBytes(destinationPath, www.bytes);
                    else ShowMsg(www.error);
                }
#else
                File.Copy(sourcePath, destinationPath, true);
#endif
                PlayerPrefs.SetString("SQLITE_" + InDbName + "_MD5", SQLite3Utility.GetFileMD5(destinationPath));
            }

            return new SQLite3Operate(destinationPath, InSQLite3OpenFlags);
        }

    }
}

