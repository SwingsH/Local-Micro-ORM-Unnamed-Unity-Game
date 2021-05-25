#if UNITY_EDITOR

using UnityEditor;
using System.IO;
using System;

[InitializeOnLoad]
public class EditorTools
{
	const string DEFINE_CLIENT_APP = "CLIENT_APP";
	const string DEFINE_SERVER_APP = "SERVER_APP";

	public static string[] definesArray;
	public static string definesString;
	public static BuildTargetGroup buildTargetGroup;

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

	public static string EditorPrefsUpdateString(string keyName, string value)
	{
		if (EditorPrefs.HasKey(keyName))
		{

			if (!String.IsNullOrWhiteSpace(value))
			{
				EditorPrefs.SetString(keyName, value);
				return value;
			}
			else if (String.IsNullOrWhiteSpace(value))
			{
				return EditorPrefs.GetString(keyName);
			}
		}
		else
		{
			EditorPrefs.SetString(keyName, value);
		}
		return value;
	}

	public static int EditorPrefsUpdateInt(string keyName, int value)
	{
		if (EditorPrefs.HasKey(keyName))
		{

			if (value != 0)
			{
				EditorPrefs.SetInt(keyName, value);
				return value;
			}
			else if (value == 0)
			{
				return EditorPrefs.GetInt(keyName);
			}
		}
		else
		{
			EditorPrefs.SetInt(keyName, value);
		}
		return value;
	}

	public static void AddScriptingDefine(string define)
	{
		if (HasScriptingDefine(define))
			return;

		PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (definesString + ";" + define));
	}

	public static void RemoveScriptingDefine(string define)
	{
		if (!HasScriptingDefine(define))
			return;

		definesArray = TIZSoft.Tools.RemoveFromArray(definesArray, define);

		definesString = string.Join(";", definesArray);

		PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (definesString));
	}

	public static bool HasScriptingDefine(string define)
	{
		buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
		definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
		definesArray = definesString.Split(';');

		if (TIZSoft.Tools.ArrayContains(definesArray, define))
			return true;

		return false;
	}
}
#endif
