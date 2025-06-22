using UnityEngine;

namespace Pelax.Utils
{
    public class Logit : MonoBehaviour
    {
        public enum LogLevels
        {
            ERROR = 0,
            WARNING = 1,
            LOG = 2,
            INFO = 3,
        }

        public LogLevels EditorLogLevel = LogLevels.INFO;
        private static LogLevels logLevel = LogLevels.INFO; // static value for editor scripts

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            // default is ERROR in release builds
            logLevel = LogLevels.ERROR;
            // if debug build, use whatever the GameObject has set
            if (Debug.isDebugBuild)
            {
                logLevel = EditorLogLevel;
            }
        }

#if UNITY_EDITOR
        // also allow dynamic update in the editor
        void Update()
        {
            if (logLevel != EditorLogLevel)
            {
                logLevel = EditorLogLevel;
            }
        }
#endif

        public static void Info(string text)
        {
            if (logLevel < LogLevels.INFO)
            {
                return;
            }
            Debug.Log(
#if UNITY_EDITOR
                "<color=cyan>[INFO]</color> " +
#else
                "[INFO] " +
#endif
                    text);
        }

        public static void Log(string text)
        {
            if (logLevel < LogLevels.LOG)
            {
                return;
            }
            Debug.Log(
#if UNITY_EDITOR
                "<color=lime>[LOG]</color> " +
#else
                "[LOG] " +
#endif
                    text);
        }

        public static void Warning(string text)
        {
            if (logLevel < LogLevels.WARNING)
            {
                return;
            }
            Debug.LogWarning(
#if UNITY_EDITOR
                "<color=yellow>[WARNING]</color> " +
#else
                "[WARNING] " +
#endif
                    text);
        }

        public static void Error(string text)
        {
            Debug.LogError(
#if UNITY_EDITOR
                "<color=red>[ERROR]</color> " +
#else
                "[ERROR] " +
#endif
                    text);
        }
    }
}
