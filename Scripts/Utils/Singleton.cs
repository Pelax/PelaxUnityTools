using System;
using UnityEngine;

namespace Pelax.Utils
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        public static Type Type
        {
            get { return typeof(T); }
        }
        private static T _instance;
        public bool EnableDontDestroyOnLoad = true;

        protected virtual void Awake()
        {
            if (IsInstanced)
            {
                Destroy(gameObject);
                return;
            }
            if (Instance.enabled)
            {
                Initialize();
            }
        }

        public virtual void Initialize()
        {
            if (EnableDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public static T Instance
        {
            get
            {
                if (!Available)
                {
                    if (Application.isPlaying)
                    {
                        GameObject obj = new GameObject($"[ {Type} INSTANCE ]");
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        public static T Available
        {
            get
            {
                if (_instance == null)
                {
                    var objects = FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
                    if (objects.Length == 0)
                    {
                        Logit.Log($"{typeof(T)} singleton no objects found!");
                        _instance = null;
                    }
                    else
                    {
                        _instance = objects[0];
                        if (objects.Length > 1)
                        {
                            Logit.Error($"Singleton: Class {Type} has multiple instances");
                            Debug.Break();
                        }
                    }
                }
                Logit.Log(
                    $"{typeof(T)} singleton Available method found nothing? " + (_instance == null)
                );
                return _instance;
            }
        }

        public static bool IsInstanced
        {
            get { return _instance != null; }
        }
    }
}
