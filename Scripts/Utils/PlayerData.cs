using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pelax.Utils
{
    /// <summary>
    /// This class is to preserve data in webgl builds, mostly because of how itch.io handles it
    /// It works on android and desktop, doesn't currently work for browsers under iOS
    /// </summary>
    public static class PlayerData
    {
        public static string BasePath = null;

        public static string PerEditorInstanceKey(string key)
        {
#if UNITY_EDITOR
            key = Application.dataPath + key;
#endif
            return key;
        }

        public static void SetInt(string key, int value)
        {
            key = PerEditorInstanceKey(key);
#if UNITY_WEBGL
            SaveData(key, value.ToString());
#else
            PlayerPrefs.SetInt(key, value);
#endif
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            key = PerEditorInstanceKey(key);
#if UNITY_WEBGL
            return int.Parse(GetData(key, defaultValue.ToString()));
#else
            return PlayerPrefs.GetInt(key, defaultValue);
#endif
        }

        public static void SetFloat(string key, float value)
        {
            key = PerEditorInstanceKey(key);
#if UNITY_WEBGL
            SaveData(key, value.ToString());
#else
            PlayerPrefs.SetFloat(key, value);
#endif
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            key = PerEditorInstanceKey(key);
#if UNITY_WEBGL
            return float.Parse(GetData(key, defaultValue.ToString()));
#else
            return PlayerPrefs.GetFloat(key, defaultValue);
#endif
        }

        public static void SetString(string key, string value)
        {
            key = PerEditorInstanceKey(key);
#if UNITY_WEBGL
            SaveData(key, value);
#else
            PlayerPrefs.SetString(key, value);
#endif
        }

        public static string GetString(string key, string defaultValue = "")
        {
            key = PerEditorInstanceKey(key);
#if UNITY_WEBGL
            return GetData(key, defaultValue.ToString());
#else
            return PlayerPrefs.GetString(key, defaultValue);
#endif
        }

        public static bool HasKey(string key)
        {
            key = PerEditorInstanceKey(key);
#if UNITY_WEBGL
            CreateBasePath();
            string fullPath = Path.Combine(BasePath, key);
            return File.Exists(fullPath);
#else
            return PlayerPrefs.HasKey(key);
#endif
        }

        private static void SaveData(string fileName, string content)
        {
            CreateBasePath();
            string fullPath = Path.Combine(BasePath, fileName);
            File.WriteAllText(fullPath, content);
            PlayerPrefs.SetString("forceSave", string.Empty);
            PlayerPrefs.Save();
        }

        private static string GetData(string fileName, string defaultContent = "")
        {
            CreateBasePath();
            string fullPath = Path.Combine(BasePath, fileName);
            if (!File.Exists(fullPath))
            {
                SaveData(fileName, defaultContent);
            }
            return File.ReadAllText(fullPath);
        }

        private static void CreateBasePath()
        {
            if (BasePath == null)
            {
                string gameUniqueName = Application.productName;
                string path = Application.persistentDataPath;
                BasePath = Path.GetDirectoryName(path);
                BasePath = Path.Combine(BasePath, gameUniqueName);
                if (!Directory.Exists(BasePath))
                {
                    Directory.CreateDirectory(BasePath);
                }
            }
        }

        public static void DeleteAll()
        {
#if UNITY_WEBGL
            CreateBasePath();
            string[] files = Directory.GetFiles(BasePath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
#else
            PlayerPrefs.DeleteAll();
#endif
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Pelax/Reset Player Prefs")]
        public static void ResetPlayerPrefs()
        {
            DeleteAll();
        }
#endif
    }
}
