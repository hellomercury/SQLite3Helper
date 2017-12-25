﻿using System.Collections.Generic;

namespace SQLite3Helper.DataStruct
{

    public abstract class SyncBase
    {
        public delegate void DlgtPropertyChanged(SyncBase InObj, string InPropertyName, object InCurrentValue, object InOldValue);

        private Dictionary<string, DlgtPropertyChanged> propertyChangedDict;

        protected SyncFactory factory;

        protected SyncBase()
        {
            factory = SyncFactory.GetOrCreateSyncFactory(GetType());

            propertyChangedDict = new Dictionary<string, DlgtPropertyChanged>();
        }

        public virtual void OnSyncOne(int InIndex, object InValue)
        {
            factory.OnSyncOne(this, InIndex, InValue);
        }

        public virtual void OnSyncAll(object[] InValues)
        {
            int length = InValues.Length;

            for (int i = 0; i < length; i++)
            {
                factory.OnSyncOne(this, i, InValues[i]);
            }
        }

        public virtual void RegisterPropertyChanged(string InPropertyName, DlgtPropertyChanged InPropertyChangedFuc)
        {
            if (propertyChangedDict.ContainsKey(InPropertyName))
                propertyChangedDict[InPropertyName] += InPropertyChangedFuc;
            else
                propertyChangedDict.Add(InPropertyName, InPropertyChangedFuc);
        }

        public virtual void UnRegisterPropertyChanged(string InPropertyName, DlgtPropertyChanged InPropertyChangedFuc)
        {
            if (propertyChangedDict.ContainsKey(InPropertyName))
                propertyChangedDict[InPropertyName] -= InPropertyChangedFuc;
        }

        public virtual void OnPropertyChanged(string InPropertyName, object InPropertyValue, object InOldValue)
        {
            DlgtPropertyChanged del;
            if (propertyChangedDict.TryGetValue(InPropertyName, out del))
                del(this, InPropertyName, InPropertyValue, InOldValue);
        }
    }
}
