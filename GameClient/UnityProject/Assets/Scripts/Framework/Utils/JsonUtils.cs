using Newtonsoft.Json;

namespace Tizsoft.Utils
{
    /// <summary>
    /// 提供 JSON 工具方法。
    /// </summary>
    /// <remarks>
    /// 這是現階段隔離第三方 JSON 程式庫用的中間類別，如果要改 JSON 實作，請在這裡改。
    /// </remarks>
	public static class JsonUtils
	{
        /// <summary>
        /// 將物件序列化成 JSON 字串。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
		public static string ToJson<T>(T obj)
		{
			return ToJson(obj, false);
		}

        /// <summary>
        /// 將物件序列化成 JSON 字串。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="isPrettyPrint"></param>
        /// <returns></returns>
		public static string ToJson<T>(T obj, bool isPrettyPrint)
		{
			return JsonConvert.SerializeObject(obj, isPrettyPrint ? Formatting.Indented : Formatting.None);
		}

        /// <summary>
        /// 將 JSON 字串反序列化為物件。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
		public static T FromJson<T>(string json)
        {
	        return JsonConvert.DeserializeObject<T>(json);
        }
	}
}