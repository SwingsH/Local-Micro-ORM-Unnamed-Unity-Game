using TIZSoft.Utils;

namespace TIZSoft.SaveData
{
    /// <summary>
    /// base class of local save，todo: append more funcitons
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