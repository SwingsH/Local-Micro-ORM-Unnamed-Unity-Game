
using TIZSoft;
using TIZSoft.DebugManager;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TIZSoft.DebugManager
{
	[System.Serializable]
	public partial class DebugHelper
	{
		public bool debugMode;
		protected List<DebugProfile> debugProfiles = new List<DebugProfile>();
				
		public void Init()
		{
			if (!debugMode)
				debugMode = true;
		}

		public void Log(string message)
		{
			if (debugMode)
				UnityEngine.Debug.Log(message);
		}

		public void LogWarning(string message)
		{
			if (debugMode)
				UnityEngine.Debug.LogWarning(message);
		}
		
		public void LogError(string message)
		{
			if (debugMode)
				UnityEngine.Debug.LogError(message);
		}
		
		public void StartProfile(string name)
		{
			if (!debugMode)
				return;
				
			if (HasProfile(name))
				RestartProfile(name);
			else
				AddProfile(name);
		}
		
		public void StopProfile(string name)
		{
			if (!debugMode)
				return;
				
			int index = GetProfileIndex(name);
			if (index != -1)
				debugProfiles[index].Stop();
		}
		
		public void PrintProfile(string name)
		{
			if (!debugMode)
				return;
				
			int index = GetProfileIndex(name);
			if (index != -1)
				Log(debugProfiles[index].Print);
		}
		
		public void Reset()
		{
			if (!debugMode)
				return;
				
			foreach (DebugProfile profile in debugProfiles)
				profile.Reset();
		}
		
		protected bool HasProfile(string _name)
		{
			return debugProfiles.Any(x => x.name == _name);
		}
		
		protected int GetProfileIndex(string _name)
		{
			return debugProfiles.FindIndex(x => x.name == _name);
		}
		
		protected void AddProfile(string name)
		{
			debugProfiles.Add(new DebugProfile(name));
		}
		
		protected void RestartProfile(string name)
		{
			int index = GetProfileIndex(name);
			if (index != -1)
				debugProfiles[index].Restart();
		}
	}
}
