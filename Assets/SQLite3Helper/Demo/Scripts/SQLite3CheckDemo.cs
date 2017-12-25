using System.Collections;
using System.Collections.Generic;
using SQLite3Helper;
using SQLite3Helper.DataStruct;
using UnityEngine;
using UnityEngine.Profiling;

public class SQLite3CheckDemo : MonoBehaviour
{

    SQLite3Operate operate;
    void Start()
    {
        operate = SQLite3Operate.CreateAndWrite("Static.db");

        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            log += condition + "\n\n";
        };
    }

    string log = string.Empty;
    Vector2 scrollPos = Vector2.zero;
    bool isSelectSingleData = false;
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width / 3.0f, Screen.height));
        GUI.skin.button.fontSize = 32;
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        GUI.skin.label.fontSize = 32;
        GUI.skin.label.fontStyle = FontStyle.Bold;

        if (GUILayout.Button((isSelectSingleData ? "V" : ">") + "\tCheck table exists."))
        {
            isSelectSingleData = !isSelectSingleData;
        }

        if (isSelectSingleData)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Check table exists by table name."))
            {
                PlayerInfo info = operate.SelectTByID<PlayerInfo>(6);
                Debug.LogError(info);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Check table exists by table name."))
            {
               bool result = operate.TableExists("CheckTable");
                Debug.LogError(result);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("check table exists by object."))
            {
                bool result = operate.TableExists<CheckTable>();
                Debug.LogError(result);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Drop the table named 'CheckTable'."))
            {
                operate.DropTable("CheckTable");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Creat the table named 'CheckTable'"))
            {
                operate.CreateTable<CheckTable>();
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
