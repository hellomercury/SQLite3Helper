using System.Collections;
using System.Collections.Generic;
using SQLite3Helper;
using SQLite3Helper.DataStruct;
using UnityEngine;
using UnityEngine.Profiling;

public class SQLite3WriteDemo : MonoBehaviour
{

    SQLite3Operate operate;
    void Start()
    {
        operate = SQLite3Operate.LoadToWrite("Write.db");

        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            log += condition + "\n\n";
        };
    }

    string log = string.Empty;
    Vector2 scrollPos = Vector2.zero;
    bool isCreate = false, isInsert = false, isDelete = false, isDrop = false;
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width / 3, Screen.height));
        GUI.skin.button.fontSize = 32;
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        GUI.skin.label.fontSize = 32;
        GUI.skin.label.fontStyle = FontStyle.Bold;

        /************************************* Create ***********************************************************/
        if (GUILayout.Button((isCreate ? "V" : ">") + "\tCreate Table."))
        {
            isCreate = !isCreate;
            if (isCreate)
            {
                isInsert = false;
                isDelete = false;
                isDrop = false;
            }
        }

        if (isCreate)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Create by sql command"))
            {
                if (operate.CreateTable("DROP TABLE IF EXISTS SQLCommandTemp; CREATE TABLE SQLCommandTemp(ID INTEGER PRIMARY KEY, Name TEXT)"))
                    Debug.LogError("Create table SQLCommandTemp successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Create by name and ‘Fields constraints’"))
            {
                if (operate.CreateTable("KeyValueTemp", new string[] { "ID INTEGER PRIMARY KEY", "Name Text" }))
                    Debug.LogError("Create table KeyValueTemp successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Create by subclass of SyncBase."))
            {
                if (operate.CreateTable<PlayerInfo>())
                    Debug.LogError("Create table Item successed.");
            }
            GUILayout.EndHorizontal();
        }

        /************************************* Drop ***********************************************************/
        if (GUILayout.Button((isDrop ? "V" : ">") + "\tDrop Table"))
        {
            isDrop = !isDrop;
            if (isDrop)
            {
                isCreate = false;
                isInsert = false;
                isDelete = false;
            }
        }

        if (isDrop)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Drop by name"))
            {
                if (operate.DropTable("PlayerInfo"))
                    Debug.LogError("DROP TABLE PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Drop by subclass of SyncBase"))
            {
                if (operate.DropTable<PlayerInfo>())
                    Debug.LogError("DROP TABLE PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();
        }

        /************************************* Show All Table ***********************************************************/
        if (GUILayout.Button("Show all table."))
        {

            List<object[]> objs = operate.SelectObject("name, sql", "sqlite_master",
                                                     new[] { "type" },
                                                     new[] { "=" },
                                                     new[] { "table" });
            string str = string.Empty;
            for (int i = 0; i < objs.Count; i++)
            {
                for (int j = 0; j < objs[i].Length; j++)
                {
                    str += objs[i][j] + ", ";
                }
                str += "\n";
            }
            Debug.LogError("Tables : \n" + str);

        }

        /************************************* tInsert ***********************************************************/
        if (GUILayout.Button((isInsert ? "V" : ">") + "\tInsert Data."))
        {
            isInsert = !isInsert;
            if (isInsert)
            {
                isCreate = false;
                isDelete = false;
                isDrop = false;
            }
        }

        if (isInsert)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Insert by sql command."))
            {
                if (operate.Insert("INSERT INTO PlayerInfo VALUES(9, 'Szn', 10010008)"))
                    Debug.LogError("INSERT INTO PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Insert by object."))
            {
                PlayerInfo info = new PlayerInfo(10, "Wn", 10010009, true);
                if (operate.InsertT(info))
                    Debug.LogError("INSERT INTO PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Insert by objects."))
            {
                List<PlayerInfo> infos = new List<PlayerInfo>(3);
                infos.Add(new PlayerInfo(11, "111", 10010010, true));
                infos.Add(new PlayerInfo(12, "222", 10010011, false));
                infos.Add(new PlayerInfo(13, "333", 10010012, true));
                if (operate.InsertAllT(infos))
                    Debug.LogError("INSERT INTO PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();
        }

        /************************************* Delete ***********************************************************/
        if (GUILayout.Button((isDelete ? "V" : ">") + "\tDelete Data"))
        {
            isDelete = !isDelete;
            if (isDelete)
            {
                isCreate = false;
                isInsert = false;
                isDrop = false;
            }
        }

        if (isDelete)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Delete by table name and id."))
            {
                if (operate.DeleteByID("PlayerInfo", 0)) Debug.LogError("Delete data with ID = 0.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Delete by subclass of SyncBase"))
            {
                PlayerInfo info = operate.SelectTByID<PlayerInfo>(1);
                if (operate.DeleteT(info)) Debug.LogError("Delete data with ID = 1.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Delete all data."))
            {
                if (operate.DeleteAllT<PlayerInfo>()) Debug.LogError("Clear table success.");
            }
            GUILayout.EndHorizontal();

        }

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width / 3.0f, 0, Screen.width * 2 / 3.0f, Screen.height));
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.Label(log);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void OnApplicationQuit()
    {
        operate.CloseDB();
    }
}
