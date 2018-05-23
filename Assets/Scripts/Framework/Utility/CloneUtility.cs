using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public interface ISelfClone<out T> where T : class
{
    T Clone();
}

public static class CloneUtility
{
    public static List<T> CloneListSerializable<T>(this List<T> InList)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, InList);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return formatter.Deserialize(memoryStream) as List<T>;
        }
    }

    public static Dictionary<T, List<TU>> CloneDictionary<T, TU>(Dictionary<T, List<TU>> InDictionary)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, InDictionary);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return formatter.Deserialize(memoryStream) as Dictionary<T, List<TU>>;
        }
    }

    public static List<T> CloneStructList<T>(this List<T> InList) where T : struct
    {
        if (null == InList) return null;

        int count = InList.Count;
        List<T> newList = new List<T>(count);
        for (int i = 0; i < count; ++i)
        {
            newList.Add(InList[i]);
        }

        return newList;
    }

    public static T[] CloneStructArray<T>(this T[] InArray) where T : struct
    {
        if (null == InArray) return null;

        int length = InArray.Length;
        T[] newArray = new T[length];
        for (int i = 0; i < length; ++i)
        {
            newArray[i] = InArray[i];
        }

        return newArray;
    }

    public static T[] CloneClassArray<T>(this T[] InArray) where T : class, ISelfClone<T>
    {
        if (null == InArray) return null;

        int length = InArray.Length;
        T[] newArray = new T[length];
        for (int i = 0; i < length; ++i)
        {
            newArray[i] = InArray[i].Clone();
        }

        return newArray;
    }
}