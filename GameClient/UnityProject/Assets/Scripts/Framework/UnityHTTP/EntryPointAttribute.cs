using System;

namespace TIZSoft.UnityHTTP
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EntryPointAttribute : Attribute
	{
		public string partialUrl { get; private set; }

		public EntryPointAttribute(string partialUrl)
		{
			this.partialUrl = partialUrl;
		}
	}
}