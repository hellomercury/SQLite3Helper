using System.Collections.Generic;

namespace SQLite3Helper.DataStruct
{

    public class SyncBase
    {
        public delegate void DlgtPropertyChanged(SyncBase InObj, string InPropertyName, object InCurrentValue, object InOldValue);

        private Dictionary<string, DlgtPropertyChanged> propertyChangedDict;

        protected SyncFactory factory;

        protected SyncBase()
        {
            factory = SyncFactory.GetOrCreateSyncFactory(GetType());

            propertyChangedDict = new Dictionary<string, DlgtPropertyChanged>();
        }

        public void OnSyncOne(int InIndex, object InValue)
        {
            factory.OnSyncOne(this, InIndex, InValue);
        }

        public void OnSyncAll(object[] InObject)
        {
            int length = InObject.Length;

            for (int i = 0; i < length; i++)
            {
                factory.OnSyncOne(this, i, InObject[i]);
            }
        }

        public void RegisterPropertyChanged(string InPropertyName, DlgtPropertyChanged InPropertyChangedFuc)
        {
            if (propertyChangedDict.ContainsKey(InPropertyName))
            {
                propertyChangedDict[InPropertyName] += InPropertyChangedFuc;
            }
            else
            {
                propertyChangedDict.Add(InPropertyName, InPropertyChangedFuc);
            }
        }

        public void UnRegisterPropertyChanged(string InPropertyName, DlgtPropertyChanged InPropertyChangedFuc)
        {
            if (propertyChangedDict.ContainsKey(InPropertyName))
            {
                propertyChangedDict[InPropertyName] -= InPropertyChangedFuc;
            }
        }

        public void OnPropertyChanged(string InPropertyName, object InPropertyValue, object InOldValue)
        {
            DlgtPropertyChanged del;
            if (propertyChangedDict.TryGetValue(InPropertyName, out del)
                    && del != null)
            {
                del(this, InPropertyName, InPropertyValue, InOldValue);
            }
        }
    }
}
