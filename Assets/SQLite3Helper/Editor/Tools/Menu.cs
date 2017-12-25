using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Menu : MonoBehaviour
{

    [MenuItem("Tools/Open Folder/Persistent Data Path")]
    static void OpenPersistentDataFloder()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    [MenuItem("Tools/Open Folder/Streaming Assets Path")]
    static void OpenStreamingAssetsFloder()
    {
        if (!Directory.Exists(Application.streamingAssetsPath)) Directory.CreateDirectory(Application.streamingAssetsPath);
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
