using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace SQLite3Helper.DataStruct
{
    public class SyncProperty
    {
        public Type ClassType { get; private set; }
        public string ClassName { get; private set; }
        public PropertyInfo[] Infos { get; private set; }
        public int InfosLength { get; private set; }

        public SyncProperty(Type InClassType, string InClassName, PropertyInfo[] InInfos, int InInfosLength)
        {
            ClassType = InClassType;
            ClassName = InClassName;
            Infos = InInfos;
            InfosLength = InInfosLength;
        }
    }

    public class SyncFactory
    {
        private Type classType;
        private PropertyInfo[] propertyInfos;
        private int propertyInfoLength;
        private SyncProperty Property;

        public SyncFactory(Type InType)
        {
            classType = InType;
            PropertyInfo[] infos = InType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            propertyInfoLength = infos.Length;
            propertyInfos = new PropertyInfo[propertyInfoLength];
            int syncID;
            Type syncAttrType = typeof(SyncAttribute);
            for (int i = 0; i < propertyInfoLength; i++)
            {
                Object[] attrs = infos[i].GetCustomAttributes(syncAttrType, false);
                if (1 == attrs.Length && attrs[0] is SyncAttribute)
                {
                    syncID = (attrs[0] as SyncAttribute).SyncID;
                    if (-1 < syncID && syncID < propertyInfoLength) propertyInfos[syncID] = infos[i];
                    else throw new IndexOutOfRangeException("Please set SyncAttribute to the property according to the sequence.");
                }
            }

            Property = new SyncProperty(classType, classType.Name, propertyInfos, propertyInfoLength);
        }

        public void OnSyncOne(Object InObj, int InIndex, Object InValue)
        {
            if (null == InObj) throw new ArgumentNullException();
            if (0 > InIndex || InIndex >= propertyInfoLength) throw new IndexOutOfRangeException(InIndex + "< 0 || " + InIndex + " >= " + propertyInfoLength);
            if (InObj.GetType() != classType) throw new ArgumentException("The input type not matched.");

            if (null != InValue)
            {
                PropertyInfo info = propertyInfos[InIndex];
                Object oldValue = info.GetValue(InObj, null);
                if (!InValue.Equals(oldValue))
                {
                    Object value = InValue;
                    if (null == oldValue) value = ChangeType(InValue, info.PropertyType);//Convert.ChangeType(InValue, info.PropertyType);
                    info.SetValue(InObj, value, null);
                    if (InObj is SyncBase) (InObj as SyncBase).OnPropertyChanged(info.Name, oldValue, InValue);
                }
            }
        }

        private static Dictionary<Type, SyncFactory> factoriesDict = new Dictionary<Type, SyncFactory>();
        public static SyncFactory GetOrCreateSyncFactory(Type InType)
        {
            SyncFactory factory;
            if (!factoriesDict.TryGetValue(InType, out factory))
            {
                try
                {
                    factory = new SyncFactory(InType);
                    factoriesDict.Add(InType, factory);
                }
                catch (Exception ex)
                {
                    Debug.LogError(InType + " Create SyncFactory Error : " + ex.Message);
                }
            }

            return factory;
        }

        public static SyncProperty GetSyncProperty(Object InObj)
        {
            return GetOrCreateSyncFactory(InObj.GetType()).Property;
        }

        public static SyncProperty GetSyncProperty(Type InType)
        {
            return GetOrCreateSyncFactory(InType).Property;
        }

        public static object ChangeType(object InObj, Type InType)
        {
            if (InType.IsValueType) return Convert.ChangeType(InObj, InType);
            else if (InType.IsArray)
            {
                Array array = null;
                int rank = InType.GetArrayRank(), firstDimension, secondDimension;
                string[] firstValue;
                string[][] secondValue;
                Type elementType;
                switch (rank)
                {
                    case 1:
                        elementType = InType.GetElementType();

                        if (elementType.IsArray)
                        {
                            Type subType = elementType.GetElementType();
                            if (1 == elementType.GetArrayRank() && !subType.IsArray)
                            {
                                if (SyncConfig.SyncArraySplit.Length < 2) throw new IndexOutOfRangeException("Please set the character of the split string first.");

                                firstValue = InObj.ToString().Split(SyncConfig.SyncArraySplit[0]);
                                firstDimension = firstValue.Length;
                                if (0 < firstDimension)
                                {
                                    string[][] elementValue = new string[firstDimension][];
                                    for (int i = 0; i < firstDimension; ++i)
                                    {
                                        elementValue[i] = firstValue[i].Split(SyncConfig.SyncArraySplit[1]);
                                    }

                                    array = Array.CreateInstance(elementType, firstDimension);
                                    Array subArray;
                                    for (int i = 0; i < firstDimension; ++i)
                                    {
                                        secondDimension = elementValue[i].Length;
                                        subArray = Array.CreateInstance(subType, secondDimension);
                                        for (int j = 0; j < secondDimension; ++j)
                                        {
                                            subArray.SetValue(Convert.ChangeType(elementValue[i][j], subType), j);
                                        }
                                        array.SetValue(subArray, i);
                                    }
                                }
                            }
                            else throw new NotSupportedException("Array type not supported");
                        }
                        else
                        {
                            if (SyncConfig.SyncArraySplit.Length < 1) throw new IndexOutOfRangeException("Please set the character of the split string first.");

                            firstValue = InObj.ToString().Split(SyncConfig.SyncArraySplit[0]);
                            firstDimension = firstValue.Length;
                            array = Array.CreateInstance(elementType, firstDimension);
                            for (int j = 0; j < firstDimension; j++)
                            {
                                array.SetValue(Convert.ChangeType(firstValue[j], elementType), j);
                            }
                        }
                        break;

                    case 2:
                        elementType = InType.GetElementType();
                        firstValue = InObj.ToString().Split(SyncConfig.SyncArraySplit[0]);
                        firstDimension = firstValue.Length;
                        secondValue = new string[firstDimension][];
                        for (int i = 0; i < firstDimension; ++i)
                        {
                            secondValue[i] = firstValue[i].Split(SyncConfig.SyncArraySplit[1]);
                        }
                        secondDimension = secondValue[0].Length;
                        array = Array.CreateInstance(elementType, firstDimension, secondDimension);
                        for (int i = 0; i < firstDimension; ++i)
                        {
                            for (int j = 0; j < secondDimension; ++j)
                            {
                                array.SetValue(Convert.ChangeType(secondValue[i][j], elementType), i, j);
                            }
                        }
                        break;

                    case 3:
                        elementType = InType.GetElementType();
                        firstValue = InObj.ToString().Split(SyncConfig.SyncArraySplit[0]);
                        firstDimension = firstValue.Length;
                        secondValue = new string[firstDimension][];
                        for (int i = 0; i < firstDimension; ++i)
                        {
                            secondValue[i] = firstValue[i].Split(SyncConfig.SyncArraySplit[1]);
                        }
                        secondDimension = secondValue[0].Length;
                        string[][][] thirdValue = new string[firstDimension][][];
                        for (int i = 0; i < firstDimension; ++i)
                        {
                            thirdValue[i] = new string[secondDimension][];
                            for (int j = 0; j < secondDimension; ++j)
                            {
                                thirdValue[i][j] = secondValue[i][j].Split(SyncConfig.SyncArraySplit[2]);
                            }
                        }
                        int thirdDimension = thirdValue[0][0].Length;

                        array = Array.CreateInstance(elementType, firstDimension, secondDimension, thirdDimension);
                        for (int i = 0; i < firstDimension; ++i)
                        {
                            for (int j = 0; j < secondDimension; ++j)
                            {
                                for (int k = 0; k < thirdDimension; ++k)
                                {
                                    array.SetValue(Convert.ChangeType(thirdValue[i][j][k], elementType), i, j, k);
                                }
                            }
                        }
                        break;

                    default:
                        throw new NotSupportedException("Array type not supported");
                }

                return array;
            }
            else if (InType.IsClass) return Convert.ChangeType(InObj, InType);
            else
            {
                Debug.LogError("Can not convert this type." + InType + "," + InType.IsClass + "," + InType.IsPointer + "," + InType.IsCOMObject);
                return null;
            }
        }
    }
}