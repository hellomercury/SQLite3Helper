using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SQLite3Helper.Example
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static SQLite3Operate Sqlite3ReadOperate
        {
            get
            {
                if(null == sqlite3ReadOperate)
                    sqlite3ReadOperate = SQLite3Factory.GetSQLite3Operate("Static.db", SQLite3OpenMode.ReadOnly);

                return sqlite3ReadOperate;
            }
        }
        private static SQLite3Operate sqlite3ReadOperate;
        
        public static SQLite3Operate SQLite3ReadWriteOPerate
        {
            get
            {
                if (null == sqlite3ReadWriteOperate)
                    sqlite3ReadWriteOperate = SQLite3Factory.GetSQLite3Operate("Write.db", SQLite3OpenMode.ReadWrite);

                return sqlite3ReadWriteOperate;
            }
        }
        private static SQLite3Operate sqlite3ReadWriteOperate;

        public static SQLite3Operate SQLite3OpenOrCreateOperate
        {
            get
            {
                if (null == sqlite3OpenOrCreateOperate)
                    sqlite3OpenOrCreateOperate = SQLite3Factory.GetSQLite3Operate("OpenOrCreate.db",
                        SQLite3OpenMode.OpenOrCreate);

                return sqlite3OpenOrCreateOperate;
            }
        }
        private static SQLite3Operate sqlite3OpenOrCreateOperate;

        void OnGUI()
        {
            if (GUILayout.Button("Test"))
            {
                Debug.LogError(SQLite3ReadWriteOPerate.TableExists("PlayerInfoTest"));
            }

            if (GUILayout.Button("Check"))
            {
                Debug.LogError(SQLite3OpenOrCreateOperate.TableExists("PlayerInfo"));
            }
        }
    }
}

