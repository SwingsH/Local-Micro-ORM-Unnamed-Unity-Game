using System;
using TIZSoft.SaveData;
using TIZSoft.Utils;
using UnityEngine;

namespace TIZSoft.UnknownGame.SaveData
{
    /// <summary>
    /// 表示一個遊戲資料儲存類別。
    /// </summary>
    public class UnknowGameSave
    {
        [Serializable]
        public class Settings
        {
            [Tooltip("玩家存檔的檔名。預設為 gamesave.json")]
            public string GameSaveFileName = typeof(GameSave).Name.ToLowerInvariant() + ".json";
        }

        static readonly object syncRoot = new object();

        readonly Settings settings;
        readonly SaveDataStorage saveDataStorage;

        GameSave gameSave;

        public GameSave GameSave
        {
            get
            {
                lock (syncRoot)
                {
                    if (gameSave == null)
                    {
                        gameSave = LoadGame() ?? new GameSave();
                        AddDefaultGroupIfNecessary();
                    }
                    return gameSave;
                }
            }
        }

        void AddDefaultGroupIfNecessary()
        {
            if (!gameSave.ContainsGroup(GameSave.DefaultGroupName))
            {
                gameSave.AddGroup(new GameSave.Group());
                gameSave.CurrentGroupName = GameSave.DefaultGroupName;
            }
            return;
        }

        public UnknowGameSave(Settings settings, SaveDataStorage saveDataStorage)
        {
            ExceptionUtils.VerifyArgumentNull(settings, "settings");
            ExceptionUtils.VerifyArgumentNull(saveDataStorage, "saveDataStorage");
            this.settings = settings;
            this.saveDataStorage = saveDataStorage;
        }

        public void SaveGame(bool fromWasabiiTransfer = false)
        {
            if (gameSave == null)
            {
                Debug.Log(string.Format(">>>>>>[ERROR---SaveGame()]<<<<<< gameSave == null"));
                return;
            }

            if (gameSave.CurrentUserSave == null)
            {
                Debug.Log(">>>>>>[ERROR---SaveGame()]<<<<<<CurrentUserSave == null");
                return;
            }

            int nSaveId = gameSave.CurrentUserSave.Id;
            if (nSaveId <= 0 && !fromWasabiiTransfer)
            {
                string sMsg = "";
                if (gameSave.CurrentGroupName == "default")
                    sMsg = string.Format("Skip SaveGame() Id = {0}, Group = {1}", nSaveId, gameSave.CurrentGroupName);
                else
                    sMsg = string.Format(">>>>>>[ERROR---SaveGame()]<<<<<< Id = {0}, Group = {1}", nSaveId, gameSave.CurrentGroupName);

                Debug.Log(sMsg);
                return;
            }

            bool bPass = true;
            string sPW = gameSave.CurrentUserSave.Password;
            string sText = "";
            if (string.IsNullOrEmpty(sPW))
            {
                bPass = false;
                sText = string.Format(">>>>>>[ERROR---SaveGame()]<<<<<< sPW = {0}", sPW);
            }
            else if (sPW.Length < 8)
            {
                bPass = false;
                sText = string.Format(">>>>>>[ERROR---SaveGame()]<<<<<< sPW(len < 8) = {0}", sPW);
            }

            if (!bPass && !fromWasabiiTransfer)
            {
                Debug.LogError(sText);
                return;
            }

            saveDataStorage.Save(settings.GameSaveFileName, gameSave);
        }

        public GameSave LoadGame()
        {
            return saveDataStorage.Load<GameSave>(settings.GameSaveFileName);
        }
    }
}
