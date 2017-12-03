using System.Collections;
using System.Collections.Generic;
using SQLite3Helper;
using SQLite3Helper.DataStruct;
using UnityEngine;
using UnityEngine.Profiling;

public class Demo : MonoBehaviour
{

    SQLite3Operate operate;
    void Start()
    {
        operate = SQLite3Operate.Load("Static.db", SQLite3OpenFlags.ReadOnly);

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
        GUILayout.BeginArea(new Rect(0, 0, Screen.width / 2, Screen.height));
        GUI.skin.button.fontSize = 32;
        GUI.skin.label.fontSize = 32;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        if (GUILayout.Button("Select single data."))
        {
            isSelectSingleData = !isSelectSingleData;
            if(isSelectSingleData)
            {
                isSelectArrayData = false;
                isSelectDictData = false;
            }
        }

        if (isSelectSingleData)
        {
            if (GUILayout.Button("Select * From Item By ID"))
            {
                Item item = operate.SelectTByID<Item>(20300001);
                Debug.LogError(item);
            }

            //Only in the absence of primary key or primary key type is not an integer
            //GUILayout.Label("SELECT * FROM Item by index only in the absence of primary key or primary key type is not an integer");
            if (GUILayout.Button("Select * From Item By Index"))
            {
                Profiler.BeginSample("0");
                Item item = operate.SelectTByIndex<Item>(1);
                Profiler.EndSample();
                if (null == item) Debug.LogError("Error: SELECT * FROM Item by index only in the absence of primary key or primary key type is not an integer!");
                else Debug.LogError(item);
            }

            if (GUILayout.Button("Select * From Item By KeyValue"))
            {
                Profiler.BeginSample("1");
                Item item = operate.SelectTByKeyValue<Item>((int)ItemEnum.ID, 20300001);
                Profiler.EndSample();
                Debug.LogError(item);
            }

            if (GUILayout.Button("Select * From Item By ID"))
            {
                Profiler.BeginSample("2");
                Item item = operate.SelectTByKeyValue<Item>(ItemEnum.Name.ToString(), "Laser Gun");
                Profiler.EndSample();
                Debug.LogError(item);
            }

        }

        if (GUILayout.Button("Select array data"))
        {
            isSelectArrayData = !isSelectArrayData;
            if(isSelectArrayData)
            {
                isSelectSingleData = false;
                isSelectDictData = false;
            }
        }

        if (isSelectArrayData)
        {

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


        }

        if (GUILayout.Button("Select Dictionary data"))
        {
            isSelectDictData = !isSelectDictData;
            if(isSelectDictData)
            {
                isSelectSingleData = false;
                isSelectArrayData = false;
            }
        }

        if (isSelectDictData)
        {
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
        }

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height));
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.Label(log);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}
