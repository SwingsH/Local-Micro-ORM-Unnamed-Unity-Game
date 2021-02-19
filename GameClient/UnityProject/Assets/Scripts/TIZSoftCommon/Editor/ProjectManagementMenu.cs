using UnityEditor;
using UnityEngine;

namespace TIZSoft
{
    static class ProjectManagementMenu
    {
        [MenuItem("Project Management/Enable Force Sync Project", true, 1000)]
        static bool ValidateEnableForceSyncProject()
        {
            return !ProjectManagementUtils.IsForceSyncProjectEnabled;
        }

        [MenuItem("Project Management/Enable Force Sync Project", false, 1000)]
        static void EnableForceSyncProject()
        {
            ProjectManagementUtils.IsForceSyncProjectEnabled = true;
            Debug.Log("Force sync project enabled.");
        }

        [MenuItem("Project Management/Disable Force Sync Project", true, 1001)]
        static bool ValidateDisableForceSyncProject()
        {
            return ProjectManagementUtils.IsForceSyncProjectEnabled;
        }

        [MenuItem("Project Management/Disable Force Sync Project", false, 1001)]
        static void DisableForceSyncProject()
        {
            ProjectManagementUtils.IsForceSyncProjectEnabled = false;
            Debug.Log("Force sync project disabled.");
        }

        [MenuItem("Project Management/Open Original Unity Project", false, 1100)]
        static void OpenOriginalProject()
        {
            ProjectManagementUtils.OpenOriginalProject();
        }

        [MenuItem("Project Management/Open Android Unity Project", false, 1101)]
        static void OpenAndroidProject()
        {
            ProjectManagementUtils.OpenAndroidProject();
        }

        [MenuItem("Project Management/Open iOS Unity Project", false, 1102)]
        static void OpenIosProject()
        {
            ProjectManagementUtils.OpenIosProject();
        }

        [MenuItem("Project Management/Generate All Platform Unity Projects", false, 1200)]
        static void GenerateAllPlatformProjects()
        {
            ProjectManagementUtils.GenerateAllPlatformProjects();
        }

        [MenuItem("Project Management/Generate Android Unity Project", false, 1201)]
        static void GenerateAndroidProject()
        {
            ProjectManagementUtils.GenerateAndroidProject();
        }

        [MenuItem("Project Management/Generate iOS Unity Project", false, 1202)]
        static void GenerateIosProject()
        {
            ProjectManagementUtils.GenerateIosProject();
        }

        [MenuItem("Project Management/Sync All Platform Unity Projects", false, 1300)]
        static void SyncAllPlatformProjects()
        {
            ProjectManagementUtils.SyncAllPlatformProjects();
        }

        [MenuItem("Project Management/Sync Android Unity Project", false, 1301)]
        static void SyncAndroidProject()
        {
            ProjectManagementUtils.SyncAndroidProject();
        }

        [MenuItem("Project Management/Sync iOS Unity Project", false, 1302)]
        static void SyncIosProject()
        {
            ProjectManagementUtils.SyncIosProject();
        }
    }
}
