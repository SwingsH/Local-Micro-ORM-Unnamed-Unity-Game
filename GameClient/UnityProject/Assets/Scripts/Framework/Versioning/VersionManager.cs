using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TIZSoft.Versioning
{
    /// <summary>
    ///     提供一些 Unity Player Runtime 無法取得的版號資訊。
    ///     例如 Android 的 versionCode 和 iOS 的 buildNumber。
    /// </summary>
    public class VersionManager
    {
        public const int MajorIndex = 0;
        public const int MinorIndex = 1;
        public const int PatchIndex = 2;
        public const int IdentifierIndex = 3;
        
        VersionSettings versionSettings;

        /// <summary>
        ///     取得顯示用的版號。<br />
        ///     在 Android 等同於 android:versionName。<br />
        ///     在 iOS 等同於 CFBundleShortVersionString。
        /// </summary>
        public string DisplayVersion
        {
            get { return Application.version; }
        }

        /// <summary>
        ///     取得真正在使用的建置版號。<br />
        ///     在 Android 等同於 android:versionCode 轉成以 "." 作區隔的版號字串。<br />
        ///     在 iOS 等同於 CFBundleVersion。
        /// </summary>
        public string BuildVersion
        {
            get { return versionSettings != null ? versionSettings.BuildVersion : "0"; }
        }

        /// <summary>
        ///     取得真正在使用的建置版號。<br />
        ///     在 Android 等同於 android:versionCode。<br />
        ///     在 iOS 等同於 CFBundleVersion 轉成整數。
        /// </summary>
        public int BuildVersionCode
        {
            get { return versionSettings != null ? versionSettings.BuildVersionCode : 0; }
        }

        public VersionManager(VersionSettings versionSettings)
        {
            this.versionSettings = versionSettings;
        }

        public static IList<string> GetVersionComponents(string version)
        {
            var components = version.Split('.', '-').ToList();

            // Major, Minor, Patch
            while (components.Count < 3)
            {
                components.Add("0");
            }

            // Identifier
            if (components.Count <= 3)
            {
                components.Add(string.Empty);
            }

            return components;
        }

        public static string ComposeVersionString(IList<string> components)
        {
            var versionString = string.Format("{0}.{1}.{2}",
                components[MajorIndex], components[MinorIndex], components[PatchIndex]);
            if (components.Count > 3 && !string.IsNullOrEmpty(components[3]))
            {
                versionString = string.Concat(versionString, "-", components[3]);
            }
            return versionString;
        }

        public static int ComputeVersionCode(string version)
        {
            return ComputeVersionCode(version.Split('.'));
        }

        public static int ComputeVersionCode(IList<string> components)
        {
            if (components.Count < 3)
            {
                return -1;
            }

            int major, minor, patch;
            int.TryParse(components[MajorIndex], out major);
            int.TryParse(components[MinorIndex], out minor);
            int.TryParse(components[PatchIndex], out patch);
            return major*1000*1000 + minor*1000 + patch;
        }
    }
}