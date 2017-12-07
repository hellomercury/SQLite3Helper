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
        operate = SQLite3Operate.CreateAndWrite("CheckDemo.db");

        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            log += condition + "\n\n";
        };
    }

    string log = string.Empty;
    Vector2 scrollPos = Vector2.zero;
    bool isSelectSingleData = false, isSelectArrayData = false, isSelectDictData;
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
            if (isSelectSingleData)
            {
                isSelectArrayData = false;
                isSelectDictData = false;
            }
        }

        if (isSelectSingleData)
        {
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

        if (GUILayout.Button((isSelectArrayData ? "V" : ">") + "\tSelect array data"))
        {
            isSelectArrayData = !isSelectArrayData;
            if (isSelectArrayData)
            {
                isSelectSingleData = false;
                isSelectDictData = false;
            }
        }

        if (isSelectArrayData)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Select * From Item Array by Indexes"))
            {
                Profiler.BeginSample("3");
                Item[] items = operate.SelectArrayT<Item>(new int[] { (int)ItemEnum.ID }, new string[] { ">" }, new object[] { 20300001 });
                Profiler.EndSample();
                Debug.LogError("/*-SELECT * FROM Item WHERE ID > 20300001.-*/");
                foreach (var itor in items)
                {
                    Debug.LogError(itor);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Select * From Item Dictionary by Name"))
            {
                Profiler.BeginSample("4");
                Item[] items = operate.SelectArrayT<Item>(new string[] { ItemEnum.ID.ToString() }, new string[] { ">" }, new object[] { 20300001 });
                Profiler.EndSample();
                Debug.LogError("/*-SELECT * FROM Item WHERE ID > 20300001.-*/");
                foreach (var itor in items)
                {
                    Debug.LogError(itor);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Select * From Item Array by sql command"))
            {
                Profiler.BeginSample("5");
                Item[] items = operate.SelectArrayT<Item>("ID > 20300001 AND ID < 20300004");
                Profiler.EndSample();
                Debug.LogError("/*-SELECT * FROM Item WHERE ID > 20300001 AND ID < 20300004.-*/");
                foreach (var itor in items)
                {
                    Debug.LogError(itor);
                }
            }
            GUILayout.EndHorizontal();

        }

        if (GUILayout.Button((isSelectDictData ? "V" : ">") + "\tSelect Dictionary data"))
        {
            isSelectDictData = !isSelectDictData;
            if (isSelectDictData)
            {
                isSelectSingleData = false;
                isSelectArrayData = false;
            }
        }

        if (isSelectDictData)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Select * From Item Dictionary by ID"))
            {
                Profiler.BeginSample("6");
                Dictionary<int, Item> itemDict = operate.SelectDictT<Item>(new int[] { (int)ItemEnum.ID }, new string[] { ">" }, new object[] { 20300001 });
                Profiler.EndSample();
                foreach (var itor in itemDict)
                {
                    Debug.LogError(itor.Key + ":" + itor.Value);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Select * From Item Dictionary by Name"))
            {
                Profiler.BeginSample("7");
                Dictionary<int, Item> itemDict = operate.SelectDictT<Item>(new string[] { ItemEnum.ID.ToString() }, new string[] { ">" }, new object[] { 20300001 });
                Profiler.EndSample();
                foreach (var itor in itemDict)
                {
                    Debug.LogError(itor.Key + ":" + itor.Value);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(64);
            if (GUILayout.Button("Select * From Item Dictionary by sql command"))
            {
                Profiler.BeginSample("8");
                Dictionary<int, Item> itemDict = operate.SelectDictT<Item>("ID >  20300001 AND ID < 20300004");
                Profiler.EndSample();
                foreach (var itor in itemDict)
                {
                    Debug.LogError(itor.Key + ":" + itor.Value);
                }
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
