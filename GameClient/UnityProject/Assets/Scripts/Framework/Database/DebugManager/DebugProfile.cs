
using TIZSoft;
using TIZSoft.DebugManager;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TIZSoft.DebugManager
{
	public partial class DebugProfile
	{
		
		public string name;
		public Stopwatch stopWatch = new Stopwatch();
		
		protected List<long> operations = new List<long>();
		protected long lastOperation;
		
		protected bool active;
		
		public DebugProfile(string _name)
		{
			name = _name;
			Restart();
		}
		
		public void Stop()
		{
			stopWatch.Stop();
			lastOperation = stopWatch.ElapsedMilliseconds;
			operations.Add(lastOperation);
			active = false;
		}
		
		public void Restart()
		{
			lastOperation = 0;
			stopWatch.Stop();
			Start();
		}
		
		public void Reset(string _name="")
		{
			
			if (!String.IsNullOrWhiteSpace(_name))
				name = _name;
				
			operations.Clear();
			lastOperation = 0;
			stopWatch.Stop();
		}
		
		public string Print
		{
			get
			{
				CheckActive();
				return "[DebugProfile] '"+name+"' ~"+lastOperation.ToString()+"ms (~"+GetAverage.ToString()+"ms average)";
			}
		}
		
		protected void Start()
		{
			stopWatch.Start();
			active = true;
		}
		
		protected void CheckActive()
		{
			if (active)
				Stop();
		}

		protected long GetAverage
		{
			get {
				long average = 0;
				foreach (long operation in operations)
					average += operation;
				average /= operations.Count;
				return average;
			}
		}
	}
}