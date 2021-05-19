using UnityEditor;
using System.IO;

public class VersionDefine
{
	const string DEFINE_CLIENT_APP = "CLIENT_APP";
	const string DEFINE_SERVER_APP = "SERVER_APP";

	private static BuildTargetGroup[] allTargetGroups = new BuildTargetGroup[]
		{
			BuildTargetGroup.Android,
			BuildTargetGroup.iOS,
			BuildTargetGroup.Standalone,
		};

//#if !DEFINE_CLIENT_APP
	[MenuItem("SwitchApp/Set to Client APP")]
	public static void SwitchToClientApp()
	{
		PlayerSettings.companyName = "tizsoft";
		PlayerSettings.productName = "volleyballjr.s2";
		PlayerSettings.applicationIdentifier = "com.tizsoft.volleyballjr.s2";
		PlayerSettings.bundleVersion = "1.0.0";
		PlayerSettings.Android.bundleVersionCode = 200; //Android specific
		PlayerSettings.Android.keystoreName = GetKeyStorePath();
		PlayerSettings.iOS.buildNumber = "200";

		PlayerSettings.applicationIdentifier = "com.tizsoft.speedrunning";

		RemoveDefineAllBuildTarget();
		BuildTargetGroup[] targetGroups = allTargetGroups;
		foreach (BuildTargetGroup targetGroup in targetGroups)
			AddDefine(targetGroup, DEFINE_CLIENT_APP);
	}

//#elif !DEFINE_SERVER_APP
    [MenuItem("SwitchApp/Set to Server APP")]
	public static void SwitchToServerApp()
	{
		RemoveDefineAllBuildTarget();
		BuildTargetGroup[] targetGroups = allTargetGroups;
		foreach (BuildTargetGroup targetGroup in targetGroups)
			AddDefine(targetGroup, DEFINE_SERVER_APP);

	}
//#endif

	static void RemoveDefineAllBuildTarget()
	{
		BuildTargetGroup[] targetGroups = allTargetGroups;
		foreach (BuildTargetGroup targetGroup in targetGroups)
		{
			RemoveDefine(targetGroup, DEFINE_CLIENT_APP);
			RemoveDefine(targetGroup, DEFINE_SERVER_APP);

		}
	}
	public static void AddDefine(BuildTargetGroup targetGroup, string keyWord)
	{
		string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
		if (!defineSymbols.Contains(keyWord))
		{
			defineSymbols += ";" + keyWord;
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbols);
		}
	}
	public static void RemoveDefine(BuildTargetGroup targetGroup, string keyWord)
	{
		string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
		if (defineSymbols.Contains(keyWord))
		{
			defineSymbols = defineSymbols.Replace(keyWord + ";", string.Empty);
			defineSymbols = defineSymbols.Replace(keyWord, string.Empty);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbols);
		}
	}

	static void SwapAssetsInResourceFolder(string targetDirectory, string oldDirectory, string resourceDirectory)
	{
		if (Directory.Exists(oldDirectory))
		{
			Directory.Delete(oldDirectory);
		}

		if (Directory.Exists(targetDirectory))
		{
			Directory.Move(resourceDirectory, oldDirectory);
			Directory.Move(targetDirectory, resourceDirectory);
		}
	}

	public static string GetKeyStorePath()
    {
		return string.Empty;
    }
}