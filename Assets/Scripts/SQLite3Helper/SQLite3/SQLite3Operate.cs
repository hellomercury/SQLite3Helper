﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using SQLite3DbHandle = System.IntPtr;
using SQLite3Statement = System.IntPtr;
using Object = System.Object;
using SQLite3Helper.DataStruct;
using System.IO;

namespace SQLite3Helper
{
    public class SQLite3Operate
    {
        /// <summary>
        /// SQLite3 data handle for operating the database.
        /// </summary>
        private SQLite3DbHandle handle;

        /// <summary>
        /// The StringBuilder used to connect the string.
        /// </summary>
        private StringBuilder stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SQLite3Helper.SQLite3Operate"/> class.
        /// </summary>
        /// <param name="InDatabasePath">In database path.</param>
        /// <param name="InSQLite3OpenFlags">In SQLite3 open flags.</param>
        private SQLite3Operate(string InDatabasePath, SQLite3OpenFlags InSQLite3OpenFlags)
        {
            Assert.IsFalse(string.IsNullOrEmpty(InDatabasePath), "Database path can not be null.");

            if (SQLite3Result.OK == SQLite3.Open(ConvertStringToUTF8Bytes(InDatabasePath),
                out handle, (int)InSQLite3OpenFlags, IntPtr.Zero))
            {
                stringBuilder = new StringBuilder(1024);
            }
            else
            {
                SQLite3.Close(handle);
                handle = IntPtr.Zero;
                throw new FileLoadException("Database failed to open.");
            }
        }

        /// <summary>
        /// Open a database just to read data.
        /// If database is not exist there will throw a FileLoadException.
        /// And you can not write data to the database.
        /// </summary>
        /// <returns>The to read.</returns>
        /// <param name="InDbName">In db name.</param>
        public static SQLite3Operate LoadToRead(string InDbName)
        {
            return Load(InDbName, SQLite3OpenFlags.ReadOnly);
        }

        /// <summary>
        /// Open a exist database to write and read data.
        /// If database is not exist there will throw a FileLoadException.
        /// </summary>
        /// <returns>The to write.</returns>
        /// <param name="InDbName">In db name.</param>
        public static SQLite3Operate LoadToWrite(string InDbName)
        {
            return Load(InDbName, SQLite3OpenFlags.ReadWrite);
        }

        /// <summary>
        /// Create a new database to write and write data.
        /// </summary>
        /// <returns>The SQLite3Operate object.</returns>
        /// <param name="InDbName">In db name.</param>
        public static SQLite3Operate CreateAndWrite(string InDbName)
        {
            string destinationPath = Path.Combine(Application.persistentDataPath, InDbName);
            return new SQLite3Operate(destinationPath, SQLite3OpenFlags.Create | SQLite3OpenFlags.ReadWrite);
        }

        /// <summary>
        /// Copy a exist database from the StreamingAssets path to PrersistentDataPath.
        ///And open it according the open flags. 
        /// </summary>
        /// <returns>The SQLite3Operate object.</returns>
        /// <param name="InDbName">In db name.</param>
        /// <param name="InSQLite3OpenFlags">In SQLite3 open flags.</param>
        private static SQLite3Operate Load(string InDbName, SQLite3OpenFlags InSQLite3OpenFlags)
        {
            string destinationPath = Path.Combine(Application.persistentDataPath, InDbName);

            if (!File.Exists(destinationPath))
            {
                string streamPath, sourcePath;
#if UNITY_ANDROID
                streamPath = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IOS
                streamPath = Application.dataPath + "/Raw/";
#else
                streamPath = Application.streamingAssetsPath + "/";
#endif

                sourcePath = Path.Combine(streamPath, InDbName);


#if UNITY_ANDROID
                using(WWW www = new WWW(sourcePath))
                {
                    while (www.isDone){}
                    if (string.IsNullOrEmpty(www.error)) File.WriteAllBytes(destinationPath, www.bytes);
                    else Debug.LogError(www.error);
                }
#else
                File.Copy(sourcePath, destinationPath, true);
#endif
            }

            return new SQLite3Operate(destinationPath, InSQLite3OpenFlags);
        }

        /// <summary>
        ///  Check the database table exists.
        /// </summary>
        /// <returns><c>true</c>, if table exists, <c>false</c> otherwise.</returns>
        /// <typeparam name="T">The Subclass of SyncBase.</typeparam>
        public bool TablesExists<T>() where T : SyncBase
        {
            return TableExists(SyncFactory.GetSyncProperty(typeof(T)).ClassName);
        }

        /// <summary>
        /// Check the database table exists.
        /// </summary>
        /// <returns><c>true</c>, if table exists, <c>false</c> otherwise.</returns>
        /// <param name="InTableName">In table name.</param>
        public bool TableExists(string InTableName)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM sqlite_master WHERE type = 'table' AND name = '")
                         .Append(InTableName)
                         .Append("'");
            SQLite3Statement stmt = ExecuteQuery(stringBuilder.ToString());

            bool result;
            if (SQLite3Result.Row == SQLite3.Step(stmt)) result = SQLite3.ColumnCount(stmt) > 0;
            else result = false;

            SQLite3.Finalize(stmt);
            return result;
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="InSQLStetement">SQL Statement.</param>
        public void CreateTable(string InSQLStetement)
        {
            Exec(InSQLStetement);
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InColumnNameAndType">In column name and type.</param>
        public void CreateTable(string InTableName, params string[] InColumnNameAndType)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("CREATE TABLE ").Append(InTableName).Append(" (");
            int length = InColumnNameAndType.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InColumnNameAndType[i]).Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public void CreateTable<T>() where T : SyncBase
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            Exec("DROP TABLE IF EXISTS " + property.ClassName);

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("CREATE TABLE ").Append(property.ClassName).Append("(");
            int length = property.Infos.Length;
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append(property.Infos[i].Name);

                Type type = property.Infos[i].PropertyType;

                if (type == typeof(int) || type == typeof(long))
                {
                    stringBuilder.Append(" INTEGER ");
                }
                else if (type == typeof(string))
                {
                    stringBuilder.Append(" TEXT ");
                }
                else if (type == typeof(float) || type == typeof(double))
                {
                    stringBuilder.Append(" REAL ");
                }
                else
                {
                    stringBuilder.Append(" BLOB ");
                }

                object[] objs = property.Infos[i].GetCustomAttributes(typeof(SQLite3ConstraintAttribute), false);
                if (objs.Length == 1 && objs[0] is SQLite3ConstraintAttribute)
                    stringBuilder.Append((objs[0] as SQLite3ConstraintAttribute).Constraint);

                stringBuilder.Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Execute insert SQL statement.
        /// </summary>
        /// <param name="InSQLstatement">In SQL statement.</param>
        public void Insert(string InSQLstatement)
        {
            Exec(InSQLstatement);
        }

        /// <summary>
        /// Execute insert SQL statement Through the assembly parameters into SQL statements.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InData">Data inserted to the table.</param>
        public void Insert(string InTableName, params object[] InData)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("INSERT INTO ").Append(InTableName).Append(" VALUES(");

            int length = InData.Length;
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append("'")
                    .Append(InData[i].ToString().Replace("'", "''"))
                    .Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Insert subclass of SyncBase into the table.
        /// </summary>
        /// <param name="InValue">Subclass of SyncBase object.</param>
        /// <typeparam name="T">Subclass of SyncBase</typeparam>
        public void InsertT<T>(T InValue) where T : SyncBase
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("INSERT INTO ").Append(property.ClassName).Append(" VALUES(");

            int length = property.Infos.Length;
            bool needColon;
            for (int i = 0; i < length; i++)
            {
                needColon = property.Infos[i].PropertyType.IsClass;
                if (needColon) stringBuilder.Append("'");
                stringBuilder.Append(property.Infos[i].GetValue(InValue, null).ToString().Replace("'", "''"));
                if (needColon) stringBuilder.Append("'");
                stringBuilder.Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Insert some SyncBase subclasses into the table.
        /// </summary>
        /// <param name="InValue">Some SyncBase subclasses list.</param>
        /// <typeparam name="T">subclass of SyncBase.</typeparam>
        public void InsertAllT<T>(List<T> InValue) where T : SyncBase
        {
            if (null == InValue) throw new ArgumentNullException();
            int count = InValue.Count;
            if (count > 0)
            {
                SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

                for (int i = 0; i < count; ++i)
                {
                    stringBuilder.Remove(0, stringBuilder.Length);
                    stringBuilder.Append("INSERT INTO ").Append(property.ClassName).Append(" VALUES(");

                    int length = property.Infos.Length;
                    bool needColon;
                    for (int j = 0; j < length; j++)
                    {
                        needColon = property.Infos[j].PropertyType.IsClass;
                        if (needColon) stringBuilder.Append("'");
                        stringBuilder.Append(property.Infos[j].GetValue(InValue[i], null).ToString().Replace("'", "''"));
                        if(needColon) stringBuilder.Append("'");
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Remove(stringBuilder.Length - 2, 2);
                    stringBuilder.Append(")");

                    Exec(stringBuilder.ToString());
                }
            }
        }

        /// <summary>
        /// According to the SQL statement to update the table. 
        /// </summary>
        /// <param name="InSQLStatement">In SQL statement.</param>
        public void Update(string InSQLStatement)
        {
            Exec(InSQLStatement);
        }

        /// <summary>
        /// Execute update SQL statement Through the assembly parameters into SQL statements.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InCondition">Analyzing conditions.</param>
        /// <param name="InData">Data update to the table.</param>
        public void Update(string InTableName, string InCondition, params string[] InData)
        {
            if (InData.Length < 1) throw new ArgumentNullException();

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ").Append(InTableName).Append(" SET ");

            int length = InData.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InData[i]).Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(" WHERE ").Append(InCondition);

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// According to the subclass of SyncBase to update the table.
        /// </summary>
        /// <param name="InValue">SyncBase object.</param>
        /// <typeparam name="T">subclass of SyncBase</typeparam>
        public void UpdateT<T>(T InValue) where T : SyncBase
        {
            if (null == InValue) throw new ArgumentNullException();

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ").Append(property.ClassName).Append(" SET ");

            int length = property.Infos.Length;
            for (int i = 1; i < length; i++)
            {
                stringBuilder.Append(property.Infos[i].Name)
                    .Append(" = '")
                    .Append(property.Infos[i].GetValue(InValue, null).ToString().Replace("'", "''"))
                    .Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(" WHERE ID = ").Append(property.Infos[0].GetValue(InValue, null));

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// The value obtained by Key Reflection updates the table
        /// </summary>
        /// <param name="InIndex">The index of the object property.</param>
        /// <param name="InValue">SyncBase subclass object</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public void UpdateTByKeyValue<T>(int InIndex, T InValue) where T : SyncBase
        {
            if (null == InValue) throw new ArgumentNullException();
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            if (InIndex < 0 || InIndex >= property.InfosLength) throw new ArgumentOutOfRangeException();

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ")
                .Append(property.ClassName)
                .Append(" SET ")
                .Append(property.Infos[InIndex].Name)
                .Append(" = '")
                 .Append(property.Infos[InIndex].GetValue(InValue, null).ToString().Replace("'", "''"))
                .Append("' WHERE ID = ")
                .Append(property.Infos[0].GetValue(InValue, null));

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// According to the SyncBase subclass object updates the table or insert into the table.
        /// </summary>
        /// <param name="InT">SyncBase subclass object.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public void UpdateOrInsert<T>(T InT) where T : SyncBase
        {
            if (null == InT) throw new ArgumentNullException();
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            SQLite3Statement stmt;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(" WHERE ID = ")
                .Append(property.Infos[0].GetValue(InT, null));

            bool isUpdate = false;
            string sql = stringBuilder.ToString();
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, sql, GetUTF8ByteCount(sql), out stmt, IntPtr.Zero))
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                    isUpdate = SQLite3.ColumnCount(stmt) > 0;
            }
            else throw new Exception(SQLite3.GetErrmsg(handle));

            SQLite3.Finalize(stmt);

            if (isUpdate) UpdateT(InT);
            else InsertT(InT);
        }

        /// <summary>
        /// According to the ID from the table to read a piece of data.
        /// </summary>
        /// <returns>a piece of data.</returns>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InID">In ID as table key.</param>
        public Object[] SelectSingleData(string InTableName, int InID)
        {
            if (handle == IntPtr.Zero) throw new NullReferenceException("Please open database first.");

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(InTableName)
                .Append(" WHERE ID = ")
                .Append(InID);

            SQLite3Statement stmt = ExecuteQuery(stringBuilder.ToString());

            Object[] obj = null;
            if (SQLite3Result.Row == SQLite3.Step(stmt))
                obj = GetObjects(stmt, SQLite3.ColumnCount(stmt));

            SQLite3.Finalize(stmt);

            return obj;
        }

        /// <summary>
        /// According to the ID from the table to read multiple data.
        /// </summary>
        /// <returns>The multiple data.</returns>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InColumnName">In column name.</param>
        /// <param name="InOperator">In Operator</param>
        /// <param name="InCondition">In condition.</param>
        public List<Object[]> SelectMultiData(string InTableName, string InColumnName, string InOperator, string InCondition)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT ")
                .Append(InColumnName)
                .Append(" FROM ")
                .Append(InTableName)
                .Append(" WHERE ")
                .Append(InOperator)
                .Append(" ")
                .Append(InCondition);

            SQLite3Statement stmt = ExecuteQuery(stringBuilder.ToString());

            List<Object[]> obj = new List<object[]>();
            int count = SQLite3.ColumnCount(stmt);
            SQLite3Result sqlite3Result;
            while (true)
            {
                sqlite3Result = SQLite3.Step(stmt);
                if (SQLite3Result.Row == sqlite3Result)
                {
                    obj.Add(GetObjects(stmt, count));
                }
                else if (SQLite3Result.Done == sqlite3Result) break;
                else throw new Exception(SQLite3.GetErrmsg(stmt));
            }

            SQLite3.Finalize(stmt);

            return obj;
        }

        /// <summary>
        /// Resolve the database results.
        /// </summary>
        /// <returns>The objects.</returns>
        /// <param name="InStmt">In sqlite statement.</param>
        /// <param name="InCount">In result count.</param>
        private Object[] GetObjects(SQLite3Statement InStmt, int InCount)
        {
            Object[] objs = new object[InCount];

            for (int i = 0; i < InCount; ++i)
            {
                SQLite3DataType type = SQLite3.ColumnType(InStmt, i);

                switch (type)
                {
                    case SQLite3DataType.Integer:
                        objs[i] = SQLite3.ColumnInt(InStmt, i);
                        break;
                    case SQLite3DataType.Real:
                        objs[i] = SQLite3.ColumnDouble(InStmt, i);
                        break;
                    case SQLite3DataType.Text:
                        objs[i] = SQLite3.ColumnText(InStmt, i);
                        break;
                    case SQLite3DataType.Blob:
                        objs[i] = SQLite3.ColumnBlob(InStmt, i);
                        break;
                    case SQLite3DataType.Null:
                        objs[i] = null;
                        break;
                }
            }

            return objs;
        }

        /// <summary>
        /// Query the object from the database by ID.
        /// Only in the absence of primary key or primary key type is not an integer
        /// </summary>
        /// <returns>SyncBase subclass object.</returns>
        /// <param name="InID">In table id as key.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public T SelectTByID<T>(int InID) where T : SyncBase, new()
        {
            return SelectTBySQLCommand<T>("SELECT * FROM "
                              + SyncFactory.GetSyncProperty(typeof(T)).ClassName
                              + " WHERE ID = " + InID);
        }

        /// <summary>
        /// Query the object from the database by index.
        /// </summary>
        /// <returns>SyncBase subclass object.</returns>
        /// <param name="InIndex">In index as key, the index value is automatically generated by the database.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public T SelectTByIndex<T>(int InIndex) where T : SyncBase, new()
        {
            return SelectTBySQLCommand<T>("SELECT * FROM "
                              + SyncFactory.GetSyncProperty(typeof(T)).ClassName
                              + " WHERE rowid = " + (InIndex + 1));    //SQLite3 rowid begin with 1.
        }

        /// <summary>
        /// Query the object from the database by property index and perperty's value.
        /// </summary>
        /// <returns>SyncBase subclass object.</returns>
        /// <param name="InPropertyIndex">In property index, The index value is specified by the SyncAttribute.</param>
        /// <param name="InExpectedValue">Expected values.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public T SelectTByKeyValue<T>(int InPropertyIndex, object InExpectedValue) where T : SyncBase, new()
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            if (InPropertyIndex < 0 || InPropertyIndex >= property.InfosLength) throw new IndexOutOfRangeException();

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(property.ClassName)
                         .Append(" WHERE ")
                         .Append(property.Infos[InPropertyIndex].Name)
                         .Append(" = '")
                         .Append(InExpectedValue.GetType().Equals(typeof(string)) ? InExpectedValue.ToString().Replace("'", "''") : InExpectedValue)
                         .Append("'");

            return SelectTBySQLCommand<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the object from the database by property name and perperty's value.
        /// </summary>
        /// <returns>SyncBase subclass object.</returns>
        /// <param name="InPropertyName">In property name.</param>
        /// <param name="InExpectedValue">Expected values.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public T SelectTByKeyValue<T>(string InPropertyName, object InExpectedValue) where T : SyncBase, new()
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(SyncFactory.GetSyncProperty(typeof(T)).ClassName)
                         .Append(" WHERE ")
                         .Append(InPropertyName)
                         .Append(" = '")
                         .Append(InExpectedValue.GetType().Equals(typeof(string)) ? InExpectedValue.ToString().Replace("'", "''") : InExpectedValue)
                         .Append("'");

            return SelectTBySQLCommand<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the object from the database by sql statement.
        /// </summary>
        /// <returns>SyncBase subclass object.</returns>
        /// <param name="InSQLStatement">In sql statement.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public T SelectTBySQLCommand<T>(string InSQLStatement) where T : SyncBase, new()
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            SQLite3Statement stmt = ExecuteQuery(InSQLStatement);

            T t = default(T);
            if (SQLite3Result.Row == SQLite3.Step(stmt))
                t = GetT(new T(), property.Infos, stmt, property.InfosLength);
            else Debug.LogError(SQLite3.GetErrmsg(stmt));
            SQLite3.Finalize(stmt);

            return t;
        }

        /// <summary>
        /// Query the database by property indexes and expected value and return the dictionary.
        /// </summary>
        /// <returns>Returns the result of the query as a dictionary.</returns>
        /// <param name="InIndexes">property indexes, The index value is specified by the SyncAttribute.</param>
        /// <param name="InOperators">Operators between properties and expected values.</param>
        /// <param name="InExpectedValues">Expected values.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public Dictionary<int, T> SelectDictT<T>(int[] InIndexes, string[] InOperators, object[] InExpectedValues) where T : SyncBase, new()
        {
            if (null == InIndexes || null == InOperators || null == InExpectedValues) throw new ArgumentNullException();
            int length = InIndexes.Length;
            if (length != InOperators.Length || length != InExpectedValues.Length) throw new ArgumentException("Parameter length does not match.");

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            string[] propertyNames = new string[length];
            for (int i = 0; i < length; i++)
            {
                propertyNames[i] = property.Infos[InIndexes[i]].Name;
            }

            return SelectDictT<T>(propertyNames, InOperators, InExpectedValues);
        }

        /// <summary>
        /// Query the database by property names and expected value and return the dictionary.
        /// </summary>
        /// <returns>Returns the result of the query as a dictionary.</returns>
        /// <param name="InPropertyNames">property names.</param>
        /// <param name="InOperators">Operators between properties and expected values.</param>
        /// <param name="InExpectedValues">Expected values.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public Dictionary<int, T> SelectDictT<T>(string[] InPropertyNames, string[] InOperators, object[] InExpectedValues) where T : SyncBase, new()
        {
            if (null == InPropertyNames || null == InOperators || null == InExpectedValues) throw new ArgumentNullException();
            int length = InPropertyNames.Length;
            if (length != InOperators.Length || length != InExpectedValues.Length) throw new ArgumentException("Parameter length does not match.");

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InPropertyNames[i])
                             .Append(" ")
                             .Append(InOperators[i])
                             .Append(" ")
                             .Append(InExpectedValues[i])
                             .Append(" AND ");
            }
            stringBuilder.Remove(stringBuilder.Length - 5, 5);

            return SelectDictT<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the dictionary from the database by sql statement.
        /// </summary>
        /// <returns>Returns the result of the query as a dictionary.</returns>
        /// <param name="InSQLStatement">In SQL Statement</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public Dictionary<int, T> SelectDictT<T>(string InSQLStatement = "") where T : SyncBase, new()
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(" WHERE ")
                .Append(InSQLStatement);

            SQLite3Statement stmt = ExecuteQuery(stringBuilder.ToString());
            Dictionary<int, T> resultDict = new Dictionary<int, T>();
            int count = SQLite3.ColumnCount(stmt), id;
            SQLite3Result result;
            while (true)
            {
                result = SQLite3.Step(stmt);
                if (SQLite3Result.Row == result)
                {
                    T t = GetT(new T(), property.Infos, stmt, count);
                    id = (int)property.Infos[0].GetValue(t, null);
                    if (!resultDict.ContainsKey(id)) resultDict.Add(id, t);
                }
                else if (SQLite3Result.Done == result) break;
                else throw new Exception(SQLite3.GetErrmsg(stmt));
            }
            SQLite3.Finalize(stmt);

            return resultDict;
        }

        /// <summary>
        /// Query the array by property indexes and expected value and return the dictionary.
        /// </summary>
        /// <returns>Returns the result of the query as a array.</returns>
        /// <param name="InIndexes">property indexes, The index value is specified by the SyncAttribute.</param>
        /// <param name="InOperators">Operators between properties and expected values.</param>
        /// <param name="InExpectedValues">Expected values.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public T[] SelectArrayT<T>(int[] InIndexes, string[] InOperators, object[] InExpectedValues) where T : SyncBase, new()
        {
            int length = InIndexes.Length;
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            string[] propertyNames = new string[length];
            for (int i = 0; i < length; i++)
            {
                propertyNames[i] = property.Infos[InIndexes[i]].Name;
            }

            return SelectArrayT<T>(propertyNames, InOperators, InExpectedValues);
        }

        /// <summary>
        /// Query the database by property names and expected value and return the array.
        /// </summary>
        /// <returns>Returns the result of the query as a dictionary.</returns>
        /// <param name="InPropertyNames">property names.</param>
        /// <param name="InOperators">Operators between properties and expected values.</param>
        /// <param name="InExpectedValues">Expected values.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public T[] SelectArrayT<T>(string[] InPropertyNames, string[] InOperators, object[] InExpectedValues) where T : SyncBase, new()
        {
            if (null == InPropertyNames || null == InOperators || null == InExpectedValues) throw new ArgumentNullException();
            int length = InPropertyNames.Length;
            if (length != InOperators.Length || length != InExpectedValues.Length) throw new ArgumentException("Parameter length does not match.");

            stringBuilder.Remove(0, stringBuilder.Length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InPropertyNames[i])
                             .Append(" ")
                             .Append(InOperators[i])
                             .Append(" ")
                             .Append(InExpectedValues[i])
                             .Append(" AND ");
            }
            stringBuilder.Remove(stringBuilder.Length - 5, 5);

            return SelectArrayT<T>(stringBuilder.ToString());
        }

        /// <summary>
        /// Query the database by sql statement and return the array.
        /// </summary>
        /// <returns>Returns the result of the query as a array.</returns>
        /// <param name="InCondition">In Condition.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public T[] SelectArrayT<T>(string InCondition = "") where T : SyncBase, new()
        {
            stringBuilder.Remove(0, stringBuilder.Length);

            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                         .Append(property.ClassName)
                         .Append(" WHERE ")
                         .Append(InCondition);


            SQLite3Statement stmt = ExecuteQuery(stringBuilder.ToString());

            List<T> resultList = new List<T>();
            SQLite3Result sqlite3Result;
            int count = SQLite3.ColumnCount(stmt);
            while (true)
            {
                sqlite3Result = SQLite3.Step(stmt);
                if (SQLite3Result.Row == sqlite3Result)
                    resultList.Add(GetT(new T(), property.Infos, stmt, count));
                else if (SQLite3Result.Done == sqlite3Result) break;
                else throw new Exception(SQLite3.GetErrmsg(stmt));
            }

            SQLite3.Finalize(stmt);

            return resultList.ToArray();
        }

        /// <summary>
        /// Convert query result from database to SyncBase subclass object.
        /// </summary>
        /// <returns>SyncBase subclass object.</returns>
        /// <param name="InBaseSubclassObj">In SyncBase subclass object.</param>
        /// <param name="InPropertyInfos">In SyncBase subclass property infos.</param>
        /// <param name="InStmt">SQLite3 result address.</param>
        /// <param name="InCount">In sqlite3 result count.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        private T GetT<T>(T InBaseSubclassObj, PropertyInfo[] InPropertyInfos, SQLite3Statement InStmt, int InCount) where T : SyncBase, new()
        {
            Type type;
            for (int i = 0; i < InCount; ++i)
            {
                type = InPropertyInfos[i].PropertyType;

                if (typeof(int) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, SQLite3.ColumnInt(InStmt, i), null);
                }
                else if (typeof(long) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, SQLite3.ColumnInt64(InStmt, i), null);
                }
                else if (typeof(float) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, (float)SQLite3.ColumnDouble(InStmt, i), null);
                }
                else if (typeof(double) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, SQLite3.ColumnDouble(InStmt, i), null);
                }
                else if (typeof(string) == type)
                {
                    InPropertyInfos[i].SetValue(InBaseSubclassObj, SQLite3.ColumnText(InStmt, i), null);
                }
            }

            return InBaseSubclassObj;
        }

        /// <summary>
        /// Deletes the data by identifier.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        /// <param name="InID">In identifier of data.</param>
        public void DeleteByID(string InTableName, int InID)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(InTableName)
                .Append(" WHERE ID = ")
                .Append(InID);

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //rebuild the built-in index.
        }

        /// <summary>
        /// Deletes the data by SyncBase subclass object.
        /// </summary>
        /// <param name="InID">In Subclass object id.</param>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public void DeleteT<T>(T InID) where T : SyncBase
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(property.ClassName)
                .Append(" WHERE ID = ")
                .Append(property.Infos[0].GetValue(InID, null));

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //rebuild the built-in index.
        }

        /// <summary>
        /// Clear table data by SyncBase of subclass.
        /// </summary>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public void DeleteAllT<T>() where T : SyncBase
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(T));

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DELETE FROM ")
                .Append(property.ClassName);

            Exec(stringBuilder.ToString());

            Exec("VACUUM");    //rebuild the built-in index.
        }

        /// <summary>
        /// Drop the table.
        /// </summary>
        /// <typeparam name="T">Subclass of SyncBase.</typeparam>
        public void DropTable<T>()
        {
            DropTable(SyncFactory.GetSyncProperty(typeof(T)).ClassName);
        }

        /// <summary>
        /// Drop the table.
        /// </summary>
        /// <param name="InTableName">In table name.</param>
        public void DropTable(string InTableName)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("DROP TABLE ")
                         .Append(InTableName);

            Exec(stringBuilder.ToString());
        }

        /// <summary>
        /// Executed the SQL statement and return the address of sqlite3.
        /// </summary>
        /// <returns>the address of sqlite3.</returns>
        /// <param name="InSQLStatement">In sql statement.</param>
        private SQLite3Statement ExecuteQuery(string InSQLStatement)
        {
            Debug.LogError(InSQLStatement);
            SQLite3Statement stmt = IntPtr.Zero;

            try
            {
                if (SQLite3Result.OK == SQLite3.Prepare2(handle, InSQLStatement, GetUTF8ByteCount(InSQLStatement), out stmt, IntPtr.Zero))
                    return stmt;
                else
                    throw new Exception(InSQLStatement + " Error: \n" + SQLite3.GetErrmsg(stmt));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                SQLite3.Finalize(stmt);
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Executed the SQL statement.
        /// </summary>
        /// <returns>The exec.</returns>
        /// <param name="InSQLStatement">In SQL Statement.</param>
        public void Exec(string InSQLStatement)
        {
            Debug.LogError(InSQLStatement);
            SQLite3Statement stmt = IntPtr.Zero;

            try
            {
                if (SQLite3Result.OK == SQLite3.Prepare2(handle, InSQLStatement, GetUTF8ByteCount(InSQLStatement), out stmt, IntPtr.Zero))
                {
                    if (SQLite3Result.Done != SQLite3.Step(stmt))
                        throw new Exception(SQLite3.GetErrmsg(stmt));
                }
                else throw new Exception(SQLite3.GetErrmsg(stmt));

            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                SQLite3.Finalize(stmt);

            }


        }

        /// <summary>
        /// Closes the database.
        /// </summary>
        public void CloseDB()
        {
            if (SQLite3DbHandle.Zero != handle)
            {
                if (SQLite3Result.OK == SQLite3.Close(handle))
                    handle = SQLite3DbHandle.Zero;
                else
                    Debug.LogError(SQLite3.GetErrmsg(handle));
            }
        }

        /// <summary>
        /// get utf8 bytes length of string.
        /// </summary>
        /// <returns>The UTF 8 bytes count.</returns>
        /// <param name="InStr">In original string.</param>
        private int GetUTF8ByteCount(string InStr)
        {
            return Encoding.UTF8.GetByteCount(InStr);
        }

        /// <summary>
        /// Converts the string to UTF 8 bytes.
        /// </summary>
        /// <returns>The string to UTF 8 bytes.</returns>
        /// <param name="InStr">In string.</param>
        private byte[] ConvertStringToUTF8Bytes(string InStr)
        {
            int length = Encoding.UTF8.GetByteCount(InStr);
            byte[] bytes = new byte[length + 1];
            Encoding.UTF8.GetBytes(InStr, 0, InStr.Length, bytes, 0);

            return bytes;
        }
    }
}