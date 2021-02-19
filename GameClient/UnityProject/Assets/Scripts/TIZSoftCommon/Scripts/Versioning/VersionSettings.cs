using UnityEngine;

namespace TIZSoft.Versioning
{
    /// <summary>
    ///     版本號相關資訊。
    /// </summary>
    [CreateAssetMenu(fileName = "version_settings", menuName = "Version Settings")]
    public class VersionSettings : ScriptableObject
    {
        [ReadOnlyProperty]
        [SerializeField]
        [Tooltip("真正的建置版本號，runtime 用，Editor 模式下用不到。")]
        string buildVersion;

        /// <summary>
        ///     取得真正在使用的建置版號。<br />
        ///     在 Android 等同於 android:versionCode 轉成以 "." 作區隔的版號字串。<br />
        ///     在 iOS 等同於 CFBundleVersion。
        /// </summary>
        public string BuildVersion
        {
            get { return buildVersion; }
        }

        /// <summary>
        ///     取得真正在使用的建置版號。<br />
        ///     在 Android 等同於 android:versionCode。<br />
        ///     在 iOS 等同於 CFBundleVersion 轉成整數。
        /// </summary>
        public int BuildVersionCode
        {
            get { return VersionManager.ComputeVersionCode(BuildVersion); }
        }
    }
}