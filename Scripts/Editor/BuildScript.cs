using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pelax.Utils
{
    public class BuildScript
    {
        public static void ExecuteGitBashCommand(string command)
        {
            Logit.Log($"will execute command: {command}");

            string bashExecutable = "C:/Program Files/Git/git-bash.exe";
#if UNITY_EDITOR_OSX
            bashExecutable = "sh";
#endif
            ProcessStartInfo processStartInfo = new ProcessStartInfo(
                bashExecutable,
                "-c \" " + command + " \""
            )
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
            };
            // make sure itchio's butle binary is in the path
#if UNITY_EDITOR_OSX
            processStartInfo.EnvironmentVariables["PATH"] = "/usr/local/bin:/usr/bin:/bin";
#endif

            var process = Process.Start(processStartInfo);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            var exitCode = process.ExitCode;

            Logit.Log($"output: {output}\nerror: {error}\nexit code: {exitCode}");

            process.Close();
        }

        private static string originalBunbleVersionLine;

#if UNITY_ANDROID
        [MenuItem("Builds/Build Android release for Play Store")]
        public static void BuildReleaseForPlayStore()
        {
            PerformBuild(false, true);
        }
#endif

#if UNITY_WEBGL
        [MenuItem("Builds/Build WebGL release")]
#elif UNITY_ANDROID
        [MenuItem("Builds/Build Android release")]
#elif UNITY_IOS
        [MenuItem("Builds/Build iOS release")]
#elif UNITY_SERVER
        [MenuItem("Builds/Build Dedicated Server release")]
#elif UNITY_WIN
        [MenuItem("Builds/Build Windows release")]
#endif
        public static void BuildRelease()
        {
            PerformBuild(false, false);
        }

#if UNITY_WEBGL
        [MenuItem("Builds/Build WebGL development")]
#elif UNITY_ANDROID
        [MenuItem("Builds/Build Android development")]
#elif UNITY_IOS
        [MenuItem("Builds/Build iOS development")]
#elif UNITY_SERVER
        [MenuItem("Builds/Build Dedicated Server development")]
#elif UNITY_WIN
        [MenuItem("Builds/Build Windows development")]
#endif
        public static void BuildDevelopment()
        {
            PerformBuild(true, false);
        }

        public static void PerformBuild(bool isDevelopment, bool buildForPlayStore)
        {
            BuildOptions buildOptions = isDevelopment
                ? BuildOptions.Development
                : BuildOptions.None;
            string gameName = Application.productName;
            string projectFolder = Path.GetDirectoryName(Application.dataPath);
            projectFolder = projectFolder.Replace('\\', '/');
            string updateCountCommand =
                @"cd "
                + projectFolder
                + " "
                + @"&& commit_count=$(git rev-list --count main) "
                + @"&& echo ""$commit_count"" > ""last_commit_count.txt""";
            ExecuteGitBashCommand(updateCountCommand);

            // set the last folder name per platform
#if UNITY_WEBGL
            string lastFolderName = gameName + "_web";
#elif UNITY_ANDROID
            string lastFolderName = gameName + "_android";
#elif UNITY_IOS
            string lastFolderName = gameName + "_ios";
#elif UNITY_SERVER
            string lastFolderName = gameName + "_server";
#elif UNITY_WIN
            string lastFolderName = gameName + "_win";
#endif
            // Set the target folder per os
            lastFolderName += isDevelopment ? "-dev" : "";
            string buildFolder = Path.Combine(projectFolder, "../builds/") + lastFolderName;
            // ensure the target folder exists
            if (!Directory.Exists(buildFolder))
                Directory.CreateDirectory(buildFolder);

            string commitCount = File.ReadLines("last_commit_count.txt").First();
            ModifyProjectSettings(GetVersion(), buildForPlayStore);

            // Get scene paths from EditorBuildSettings.scenes
            string[] scenePaths = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                scenePaths[i] = EditorBuildSettings.scenes[i].path;
            }

            // Set the build options for WebGL
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                options = buildOptions,
                scenes = scenePaths,
#if UNITY_WEBGL
                locationPathName = buildFolder,
                target = BuildTarget.WebGL,
#elif UNITY_ANDROID
                locationPathName = buildFolder + "/" + gameName + "_android.apk",
                target = BuildTarget.Android,
#elif UNITY_IOS
                locationPathName = buildFolder,
                target = BuildTarget.iOS,
#elif UNITY_SERVER
                locationPathName = buildFolder + "/ServerBuild",
                target = BuildTarget.StandaloneLinux64,
                targetGroup = BuildTargetGroup.Standalone,
                subtarget = (int)StandaloneBuildSubtarget.Server
#elif UNITY_WIN
                locationPathName = buildFolder,
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
#endif
            };

#if UNITY_ANDROID
            if (buildForPlayStore)
            {
                // Set Android build to generate AAB for Google Play Store
                EditorUserBuildSettings.buildAppBundle = true;
                buildPlayerOptions.locationPathName = buildFolder + "/" + gameName + ".aab";
            }
            else
            {
                EditorUserBuildSettings.buildAppBundle = false;
            }
#endif

            // Perform the WebGL build
            BuildPipeline.BuildPlayer(buildPlayerOptions);

            RestoreProjectSettings();
        }

        private static void ModifyProjectSettings(
            string buildVersion,
            bool increaseAndroidBundleVersionCode
        )
        {
            string projectSettingsPath = "ProjectSettings/ProjectSettings.asset";
            string[] projectSettingsLines = File.ReadAllLines(projectSettingsPath);
            bool foundBundleVersionLine = false;

            for (int i = 0; i < projectSettingsLines.Length; i++)
            {
                if (projectSettingsLines[i].StartsWith("  bundleVersion: "))
                {
                    originalBunbleVersionLine = projectSettingsLines[i];
                    projectSettingsLines[i] = "  bundleVersion: " + buildVersion;
                    foundBundleVersionLine = true;
                }
                if (increaseAndroidBundleVersionCode)
                {
                    if (projectSettingsLines[i].StartsWith("  AndroidBundleVersionCode: "))
                    {
                        var versionCode = int.Parse(projectSettingsLines[i].Substring(28));
                        Logit.Log($"AndroidBundleVersionCode: {versionCode}");
                        projectSettingsLines[i] =
                            "  AndroidBundleVersionCode: " + (versionCode + 1);
                        Logit.Log($"stuff written: {projectSettingsLines[i]}");
                    }
                }
            }

            if (!foundBundleVersionLine)
            {
                Logit.Error("Failed to find 'bundleVersion' line in ProjectSettings.asset");
                return;
            }

            File.WriteAllLines(projectSettingsPath, projectSettingsLines);
        }

        /// <summary>
        /// This only restores the bundle version to the original value, it lets int the play store change
        /// </summary>
        private static void RestoreProjectSettings()
        {
            if (originalBunbleVersionLine == null)
            {
                Logit.Error("Original bundle version line not stored");
                return;
            }

            string projectSettingsPath = "ProjectSettings/ProjectSettings.asset";
            string[] projectSettingsLines = File.ReadAllLines(projectSettingsPath);

            for (int i = 0; i < projectSettingsLines.Length; i++)
            {
                if (projectSettingsLines[i].StartsWith("  bundleVersion: "))
                {
                    projectSettingsLines[i] = originalBunbleVersionLine;
                    break;
                }
            }

            File.WriteAllLines(projectSettingsPath, projectSettingsLines);

            // Clear the stored original bundle version
            originalBunbleVersionLine = null;
        }

        private static string GetVersion()
        {
            string projectFolder = Path.GetDirectoryName(Application.dataPath);
            projectFolder = projectFolder.Replace('\\', '/');
            string updateCountCommand =
                @"cd "
                + projectFolder
                + " "
                + @"&& commit_count=$(git rev-list --count HEAD) "
                + @"&& echo ""$commit_count"" > ""last_commit_count.txt""";
            ExecuteGitBashCommand(updateCountCommand);
            string commitCount = File.ReadLines("last_commit_count.txt").First();
            var commitHashInt = int.Parse(commitCount);
            // commitHashInt -= 685; // the start of 0.2
            return "0.1." + commitHashInt.ToString();
        }

        [MenuItem("Builds/Upload to itch.io (development)")]
        public static void UploadItchioDevelopmentBuild()
        {
            UploadItchioBuild(true);
        }

        [MenuItem("Builds/Upload to itch.io (release)")]
        public static void UploadItchioReleaseBuild()
        {
            UploadItchioBuild(false);
        }

        public static void UploadItchioBuild(bool isDevelopment)
        {
            string channelAndFolderSuffix = isDevelopment ? "-dev" : "";
            string itchioChannel = "android" + channelAndFolderSuffix;
            string gameName = Application.productName;
            var version = GetVersion();
            string projectFolder = Path.GetDirectoryName(Application.dataPath);
            projectFolder = projectFolder.Replace('\\', '/');
            // defult values are android
            string buildFolder =
                projectFolder + "/../builds/" + gameName + "_android" + channelAndFolderSuffix;
            string uploadCommand =
                @"cd "
                + buildFolder
                + " "
                + @"&& butler push "
                + gameName
                + @"_android.apk ""pelax/"
                + gameName
                + ":"
                + itchioChannel
                + @""" --userversion "
                + version;
#if UNITY_WEBGL
            itchioChannel = "html" + channelAndFolderSuffix;
            buildFolder =
                projectFolder + "/../builds/" + gameName + "_web" + channelAndFolderSuffix;
            uploadCommand =
                @"cd "
                + buildFolder
                + " "
                + @"&& butler push . ""pelax/"
                + gameName
                + ":"
                + itchioChannel
                + @""" --userversion "
                + version;
#endif

#if UNITY_EDITOR_WIN
            uploadCommand += @" && read "; // added a read command so we can read what the console outputs under windows
#endif
            ExecuteGitBashCommand(uploadCommand);
        }

        [MenuItem("Tools/Reset Player Prefs")]
        public static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
