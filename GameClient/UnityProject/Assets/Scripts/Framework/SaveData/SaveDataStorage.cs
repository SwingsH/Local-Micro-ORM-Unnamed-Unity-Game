using Newtonsoft.Json;
using TIZSoft.Utils;

namespace TIZSoft.SaveData
{
    /// <summary>
    /// 表示一個 SaveData 本地端儲存類別。
    /// </summary>
    public class SaveDataStorage
    {
        readonly PersistentDataStorage persistentDataStorage;

        public SaveDataStorage(PersistentDataStorage persistentDataStorage)
        {
            ExceptionUtils.VerifyArgumentNull(persistentDataStorage, "persistentDataStorage");
            this.persistentDataStorage = persistentDataStorage;
        }

        /// <summary>
        /// 將 save data 存到指定路徑。
        /// 如果 <paramref name="filename"/> 是相對路徑，則會從 <see cref="PersistentDataStorage.RootDirectoryName"/> 裡尋找。
        /// </summary>
        /// <typeparam name="TSave"></typeparam>
        /// <param name="filename"></param>
        /// <param name="save"></param>
        public void Save<TSave>(string filename, TSave save)
            where TSave : Save
        {
            ExceptionUtils.VerifyArgumentNull(save, "save");
            persistentDataStorage.SaveText(filename, save.ToString(true));
        }

        /// <summary>
        /// 從指定路徑載入 save data。
        /// 如果 <paramref name="filename"/> 是相對路徑，則會從 <see cref="PersistentDataStorage.RootDirectoryName"/> 裡尋找。
        /// </summary>
        /// <typeparam name="TSave"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public TSave Load<TSave>(string filename)
            where TSave : Save
        {
            var json = persistentDataStorage.LoadText(filename);
            return JsonUtils.FromJson<TSave>(json);
        }

        public void Load<TSave>(string filename, TSave target)
        {
            var json = persistentDataStorage.LoadText(filename);
            JsonConvert.PopulateObject(json, target);
        }
    }
}