using System.Collections;
using System.Collections.Generic;
using SQLite3Helper;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static SQLite3Operate SQLiteOperate { get; private set; }
    
    void Start()
    {
        Instance = this;
        SQLiteOperate = SQLite3Operate.LoadToRead("");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
