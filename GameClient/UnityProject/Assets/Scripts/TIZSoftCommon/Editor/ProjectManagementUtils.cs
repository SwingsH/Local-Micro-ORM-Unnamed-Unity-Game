using System;
using System.IO;
using System.Threading;
using TIZSoft.Utils;
using UnityEditor;
using UnityEngine;

namespace TIZSoft
{
    /// <summary>
    /// 用來放專案控制、管理工具。
    /// </summary>
    static class ProjectManagementUtils
    {
        const string ShellScriptRootDir = "..";

        const string ShellScript_GenerateAllUnityProjects = "generate-all-unity-projects";
        const string ShellScript_GenerateUnityProjectAndroid = "generate-unity-project-android";
        const string ShellScript_GenerateUnityProjectIos = "generate-unity-project-ios";
        const string ShellScript_SyncAllUnityProjects = "sync-all-unity-projects";
        const string ShellScript_SyncUnityProjectAndroid = "sync-unity-project-android";
        const string ShellScript_SyncUnityProjectIos = "sync-unity-project-ios";

        const string MacShellFileExtension = ".command";
        const string WindowsShellFileExtension = ".ps1";

        const string KeyName_IsForceSyncProjectEnabled = "Tizsoft.ProjectManagementMenu.IsForceSyncProjectEnabled";

        public static bool IsForceSyncProjectEnabled
        {
            get
            {
                return EditorPrefs.GetBool(KeyName_IsForceSyncProjectEnabled, false);
            }
            set
            {
                EditorPrefs.SetBool(KeyName_IsForceSyncProjectEnabled, value);
            }
        }

        /// <summary>
        /// 取得專案根目錄。
        /// </summary>
        /// <returns>The unity project folder full path.</returns>
        static string GetUnityProjectFolderFullPath()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }

        static string GetOriginalProjectFullPath()
        {
            // 取得專案根目錄的完整路徑。e.g. /Workspace/ProjectName-Android
            var projectDirFullPath = GetUnityProjectFolderFullPath();

            // 只取出專案目錄名稱。e.g. /Workspace/ProjectName-Android 只取 ProjectName-Android
            var projectDirName = Path.GetFileName(projectDirFullPath);

            // 去掉平台相關的後綴名稱。e.g. /Workspace/ProjectName-Android 擷取後變成 /Workspace/ProjectName
            var lastDashIndex = projectDirName.LastIndexOf("-", StringComparison.Ordinal);
            var projectDirNameWithourPostfix = lastDashIndex > 0
                ? projectDirName.Substring(0, projectDirName.LastIndexOf("-", StringComparison.Ordinal))
                : projectDirName;

            // 移到上一層目錄。
            var projectDirFullPathParent = Path.GetFullPath(Path.Combine(projectDirFullPath, ".."));

            return Path.Combine(projectDirFullPathParent, projectDirNameWithourPostfix);
        }

        static string GetProjectFullPathByPlatform(BuildTarget targetPlatform)
        {
            return GetOriginalProjectFullPath() + "-" + targetPlatform;
        }

        static void OpenProject(BuildTarget targetPlatform)
        {
            var projectPath = GetProjectFullPathByPlatform(targetPlatform);

            // 目標平台的專案若不存在，就產生目標平台的專案。
            if (!Directory.Exists(projectPath))
            {
                switch (targetPlatform)
                {
                    case BuildTarget.Android:
                        GenerateAndroidProject();
                        break;

                    case BuildTarget.iOS:
                        GenerateIosProject();
                        break;

                    default:
                        throw new NotSupportedException();
                }

                // 等 Unity 準備好才能開啟專案。
                var unityLockFile = Path.Combine(projectPath, "Temp/UnityLockfile");
                while (!Directory.Exists(projectPath) && File.Exists(unityLockFile))
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Generating project...please wait.", "", 0F))
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                }
                EditorUtility.ClearProgressBar();
            }

            if (IsForceSyncProjectEnabled)
            {
                switch (targetPlatform)
                {
                    case BuildTarget.Android:
                        SyncAndroidProject();
                        break;

                    case BuildTarget.iOS:
                        SyncIosProject();
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            EditorApplication.OpenProject(GetProjectFullPathByPlatform(targetPlatform));
        }

        /// <summary>
        /// Gets the shell file path.
        /// </summary>
        /// <returns>The shell file path.</returns>
        /// <param name="shellFileNameWithoutExtension">Shell file name without extension.</param>
        static string GetShellFilePath(string shellFileNameWithoutExtension)
        {
            var shellFileName = shellFileNameWithoutExtension;
#if UNITY_EDITOR_OSX
            shellFileName += MacShellFileExtension;
#elif UNITY_EDITOR_WIN
			shellFileName += WindowsShellFileExtension;
#else
			throw new System.NotSupportedException();
#endif
            return Path.Combine(ShellScriptRootDir, shellFileName);
        }

        public static void OpenOriginalProject()
        {
            EditorApplication.OpenProject(GetOriginalProjectFullPath());
        }

        public static void OpenAndroidProject()
        {
            OpenProject(BuildTarget.Android);
        }

        public static void OpenIosProject()
        {
            OpenProject(BuildTarget.iOS);
        }

        public static void GenerateAllPlatformProjects()
        {
            Execute(ShellScript_GenerateAllUnityProjects);
        }

        public static void GenerateAndroidProject()
        {
            Execute(ShellScript_GenerateUnityProjectAndroid);
        }

        public static void GenerateIosProject()
        {
            Execute(ShellScript_GenerateUnityProjectIos);
        }

        public static void SyncAllPlatformProjects()
        {
            Execute(ShellScript_SyncAllUnityProjects);
        }

        public static void SyncAndroidProject()
        {
            Execute(ShellScript_SyncUnityProjectAndroid);
        }

        public static void SyncIosProject()
        {
            Execute(ShellScript_SyncUnityProjectIos);
        }

        static void Execute(string shellScriptFileName)
        {
            var shellScriptFilePath = GetShellFilePath(shellScriptFileName);
            ShellUtils.ExecuteShellScriptFile(shellScriptFilePath);
        }
    }
}
