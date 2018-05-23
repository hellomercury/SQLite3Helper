using UnityEngine;

namespace Framework.Tools
{
    public class Singleton<T> where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (null == instance) instance = new T();
                return instance;
            }
        }
    }

    public class SingletonForMono<T> : MonoBehaviour where T : SingletonForMono<T>
    {
        public static T Instance { get; private set; }

        /// <summary>
        /// 如果重写Awake方法，请确保base.Awake()在其它代码之前执行
        ///用于初始化单例 
        /// </summary>
        public virtual void Awake()
        {
            if(default(T) == Instance) Instance = this as T;
            else
            {
                Debug.LogError("The class already has an instance. ");
                DestroyImmediate(this);
            }
        }

        /// <summary>
        /// 如果重写OnEnable方法，请确保base.OnEnable()在其它代码之前执行
        /// 用于重绑定单例
        /// </summary>
        public virtual void OnEnable()
        {
            if (default(T) == Instance) Instance = this as T;
        }
        /// <summary>
        /// 如果重写OnDisable方法，请确保base.OnDisable()在其它代码之后执行
        /// 用于销毁单例
        /// </summary>
        public virtual void OnDisable()
        {
            if (default(T) != Instance) Instance = null;
        }
    }
}

