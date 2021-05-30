using TIZSoft.Utils;

namespace TIZSoft.SaveData
{
    /// <summary>
    /// 表示一個存檔的基底類別，目前僅提供較為方便用的工具方法。
    /// </summary>
    public class Save
    {
        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool isPrettyPrint)
        {
            return JsonUtils.ToJson(this, isPrettyPrint);
        }
    }
}