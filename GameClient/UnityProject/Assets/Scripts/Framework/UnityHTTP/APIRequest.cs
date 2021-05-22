using TIZSoft.Extensions;

namespace TIZSoft.UnityHTTP
{
	public abstract class APIRequest
	{
		public string partialURL
		{
			get
			{
				return GetType().GetAttribute<EntryPointAttribute>().partialUrl;
			}
		}
	}
}