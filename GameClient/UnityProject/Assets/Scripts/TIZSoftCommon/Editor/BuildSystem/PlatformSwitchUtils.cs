using UnityEditor;

namespace TIZSoft.BuildSystem
{
    public static class PlatformSwitchUtils
    {
        /// <summary>
        /// Switchs the platform to Android.
        /// </summary>
        public static void SwitchPlatformToAndroid()
        {
            SwitchPlatform(BuildTarget.Android);
        }

        /// <summary>
        /// Switchs the platform to iOS.
        /// </summary>
        public static void SwitchPlatformToIos()
        {
            SwitchPlatform(BuildTarget.iOS);
        }

        /// <summary>
        /// 切換目標平台至 <paramref name="targetPlatform"/>。
        /// </summary>
        /// <param name="targetPlatform">Target platform.</param>
        /// <example>
        /// PlatformSwitchUtils.SwitchPlatform(BuildTarget.Android);
        /// </example>
        public static void SwitchPlatform(BuildTarget targetPlatform)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetPlatform);
        }
    }
}
