using System;

namespace TIZSoft.UnityHTTP
{
	/// <summary>
	/// e.g. [EntryPoint("user/my")] 
	/// On the server side, the URL path will automatically be listening. Not require to put any disk files and folders. 
	/// </summary>
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