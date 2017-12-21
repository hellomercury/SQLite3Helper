using System;
using System.Collections.Generic;
using System.Text;
using SQLite3Helper.DataStruct;
using UnityEngine;

public class TypeTest : MonoBehaviour
{

    void Awake()
    {
        
    }

    private int id;
    private string name;
    private double age;

    void Set<T>(int InIndex, T InT)
    {
        if (0 == InIndex)
        {
            id = Convert.ToInt32(InT);
        }
    }
    void OnGUI()
    {
        if (GUILayout.Button("[]"))
        {
            int[] array = { 1, 2, 3 };
            Debug.LogError(array.GetType() + ", " + array.GetType().GetArrayRank()
                + ", " + array.GetType().GetElementType() + ", " + array.GetType().GetElementType().IsArray);
        }

        if (GUILayout.Button("[][]"))
        {
            int[][] array = { new[] { 1, 2, 3 }, new[] { 3, 4, 5 } };
            Debug.LogError(array.GetType() + ", " + array.GetType().GetArrayRank()
                + ", " + array.GetType().GetElementType());
        }

        if (GUILayout.Button("[,]"))
        {
            int[,] array = { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } };
            Debug.LogError(array.GetType() + ", " + array.GetType().GetArrayRank()
                + ", " + array.GetType().GetElementType());
        }

        if (GUILayout.Button("[,]"))
        {
            int[,] array = { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } };
            Debug.LogError(array.GetType() + ", " + array.GetType().GetArrayRank()
                + ", " + array.GetType().GetElementType());
        }

        if (GUILayout.Button("[,,]"))
        {
            int[,,] array =
            {{{ 1, 2 },{ 3, 4 }},{{ 5, 6 },{ 7, 8 }}};
            Debug.LogError(array.GetType() + ", " + array.GetType().GetArrayRank()
                + ", " + array.GetType().GetElementType());
        }


        if (GUILayout.Button("set"))
        {
            SyncProperty property = SyncFactory.GetSyncProperty(typeof(TypeData));

            TypeData data = new TypeData();
            SyncFactory factory = SyncFactory.GetOrCreateSyncFactory(typeof(TypeData));
            factory.OnSyncOne(data, 0, 0);
            factory.OnSyncOne(data, 1, "1|2|3");
            factory.OnSyncOne(data, 2, "1@2@3|4@5@6@7");
            factory.OnSyncOne(data, 3, "1@2@3@4|5@6@7@8");
            factory.OnSyncOne(data, 4, "1$2@3$4|1$2@3$4");
            Debug.LogError(data);


            int[,,] s = { { { 1, 2 }, { 3, 4 } }, { { 1, 2 }, { 3, 4 } } };
            // 1$2@3$4|1$2@3$4
        }

        if (GUILayout.Button("Copy"))
        {
            Debug.LogError(ChangeType(new[, ,] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }));
        }
    }

    struct MyStruct
    {
        public List<int> iList;
    }

    private string ChangeType(object InValue)
    {
        return ChangeType(InValue, InValue.GetType());
    }

    private string ChangeType(object InValue, Type InType)
    {
        StringBuilder result = new StringBuilder(256);
        if (InType.IsArray)
        {
            int rank = InType.GetArrayRank(), firstDimension, secondDimension; ;
            Array array, subArray;
            switch (rank)
            {
                case 1:
                    array = InValue as Array;
                    if (null != array)
                    {
                        firstDimension = array.Length;
                        if (0 < firstDimension)
                        {
                            Type subType = array.GetValue(0).GetType();
                            if (subType.IsArray)
                            {
                                if (1 == subType.GetArrayRank())
                                {
                                    for (int i = 0; i < firstDimension; ++i)
                                    {
                                        subArray = array.GetValue(i) as Array;
                                        for (int j = 0; j < subArray.Length; ++j)
                                        {
                                            result.Append(subArray.GetValue(j)).Append(SyncConfig.SyncArraySplit[1]);
                                        }
                                        result.Remove(result.Length - 1, 1);
                                        result.Append(SyncConfig.SyncArraySplit[0]);
                                    }
                                    result.Remove(result.Length - 1, 1);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < firstDimension; ++i)
                                {
                                    result.Append(array.GetValue(i)).Append(SyncConfig.SyncArraySplit[0]);
                                }
                                result.Remove(result.Length - 1, 1);
                            }
                        }
                    }
                    break;

                case 2:
                    array = InValue as Array;
                    if (null != array)
                    {
                        firstDimension = array.GetLength(0);
                        secondDimension = array.GetLength(1);
                        for (int i = 0; i < firstDimension; ++i)
                        {
                            for (int j = 0; j < secondDimension; ++j)
                            {
                                result.Append(array.GetValue(i, j)).Append(SyncConfig.SyncArraySplit[1]);
                            }
                            result.Remove(result.Length - 1, 1);
                            result.Append(SyncConfig.SyncArraySplit[0]);
                        }
                        result.Remove(result.Length - 1, 1);
                    }
                    break;

                case 3:
                    array = InValue as Array;
                    if (null != array)
                    {
                        firstDimension = array.GetLength(0);
                        secondDimension = array.GetLength(1);
                        int thirdDimension = array.GetLength(2);
                        for (int i = 0; i < firstDimension; ++i)
                        {
                            for (int j = 0; j < secondDimension; ++j)
                            {
                                for (int k = 0; k < thirdDimension; ++k)
                                {
                                    result.Append(array.GetValue(i, j, k)).Append(SyncConfig.SyncArraySplit[2]);
                                }
                                result.Remove(result.Length - 1, 1);
                                result.Append(SyncConfig.SyncArraySplit[1]);
                            }
                            result.Remove(result.Length - 1, 1);
                            result.Append(SyncConfig.SyncArraySplit[0]);
                        }
                        result.Remove(result.Length - 1, 1);
                    }
                    break;
            }
        }
        else
        {
            if (InType.IsValueType || InType.IsEnum) result.Append(InValue);
            else result.Append("'").Append(InValue.ToString().Replace("'", "''")).Append("'");
        }

        return result.ToString();
    }

}


public class TypeData : SyncBase
{
    [Sync(0)]
    public int ID { get; set; }

    [Sync(1)]
    public int[] SingleArray { get; private set; }

    [Sync(2)]
    public int[][] DoubleArray { get; private set; }

    [Sync(3)]
    public int[,] thridArray { get; private set; }

    [Sync(4)]
    public int[,,] FourthArray { get; private set; }


    public override string ToString()
    {
        string str = ID + "\n";
        int length = SingleArray.Length;
        for (int i = 0; i < length; ++i)
        {
            str += SingleArray[i] + ",";
        }

        str += "\n";
        length = DoubleArray.Length;
        int length1;
        for (int i = 0; i < length; ++i)
        {
            length1 = DoubleArray[i].Length;
            for (int j = 0; j < length1; ++j)
            {
                str += DoubleArray[i][j] + ",";
            }
            str += "\n";
        }
        str += "\n";

        length = thridArray.GetLength(0);
        length1 = thridArray.GetLength(1);
        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < length1; ++j)
            {
                str += thridArray[i, j] + ",";
            }
            str += "\n";
        }
        str += "\n";

        length = FourthArray.GetLength(0);
        length1 = FourthArray.GetLength(1);
        int length2 = FourthArray.GetLength(2);
        for (int i = 0; i < length; ++i)
        {
            str += "-";
            for (int j = 0; j < length1; ++j)
            {
                str += "--";
                for (int k = 0; k < length2; ++k)
                {
                    str += FourthArray[i, j, k] + ",";
                }
                str += "\n";
            }
            str += "\n";
        }
        str += "\n";
        return str;
    }
}
