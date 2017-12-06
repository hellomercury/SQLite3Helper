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
            if (GUILayout.Button("Create table by sql command"))
            {
                operate.CreateTable("CREATE TABLE SQLCommandTemp(ID INTEGER PRIMARY KEY, Name TEXT)");
                Debug.LogError("Create table SQLCommandTemp successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Create table by name and ‘Fields constraints’"))
            {
                operate.CreateTable("KeyValueTemp", "ID INTEGER PRIMARY KEY", "Name Text");
                Debug.LogError("Create table KeyValueTemp successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Create table by subclass of SyncBase."))
            {
                operate.CreateTable<Item>();
                Debug.LogError("Create table Item successed.");
            }
            GUILayout.EndHorizontal();
        }

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
            if (GUILayout.Button("Insert data to table by sql command."))
            {
                operate.Insert("INSERT INTO PlayerInfo VALUES(9, 'Szn', 10010008)");
                Debug.LogError("INSERT INTO PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Insert data to table by object."))
            {
                PlayerInfo info = new PlayerInfo(10, "Wn", 10010009);
                operate.InsertT(info);
                Debug.LogError("INSERT INTO PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Insert data to table by objects."))
            {
                List<PlayerInfo> infos = new List<PlayerInfo>(3);
                infos.Add(new PlayerInfo(11, "111", 10010010));
                infos.Add(new PlayerInfo(12, "222", 10010011));
                infos.Add(new PlayerInfo(13, "333", 10010012));
                operate.InsertAllT(infos);
                Debug.LogError("INSERT INTO PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();
        }

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
            if (GUILayout.Button("Delete single data by table name and id."))
            {
                operate.DeleteByID("PlayerInfo", 0);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Delete single data by subclass of SyncBase"))
            {
                PlayerInfo info = operate.SelectTByID<PlayerInfo>(1);
                operate.DeleteT(info);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Delete a table all data."))
            {
                operate.DeleteAllT<PlayerInfo>();
            }
            GUILayout.EndHorizontal();

        }

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
            if (GUILayout.Button("Drop table by name"))
            {
                operate.DropTable("PlayerInfo");
                Debug.LogError("DROP TABLE PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Drop table by subclass of SyncBase"))
            {
                operate.DropTable<PlayerInfo>();
                Debug.LogError("DROP TABLE PlayerInfo Successed.");
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width / 3, 0, Screen.width * 2 / 2, Screen.height));
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
