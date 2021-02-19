using Newtonsoft.Json;
using System;
using UnityEngine;

namespace TIZSoft.SaveData
{
    /// <summary>
    /// 基礎系統設定存檔。
    /// </summary>
    [Serializable]
    public class SystemSave : Save
    {
        [SerializeField]
        [Range(0F, 1F)]
        [Tooltip("總音量。")]
        float audioVolume = 1F;

        [SerializeField]
        [Range(0F, 1F)]
        [Tooltip("音效音量。")]
        float sfxVolume = 1F;
        
        [SerializeField]
        [Range(0F, 1F)]
        [Tooltip("背景音樂音量。")]
        float bgmVolume = 1F;

        /// <summary>
        /// 取得或設定總音量。
        /// </summary>
        [JsonProperty("audioVolume")]
        public float AudioVolume
        {
            get { return audioVolume; }
            set { audioVolume = value; }
        }

        /// <summary>
        /// 取得或設定音效音量。
        /// </summary>
        [JsonProperty("sfxVolume")]
        public float SfxVolume
        {
            get { return sfxVolume; }
            set { sfxVolume = value; }
        }

        /// <summary>
        /// 取得或設定背景音樂音量。
        /// </summary>
        [JsonProperty("bgmVolume")]
        public float BgmVolume
        {
            get { return bgmVolume; }
            set { bgmVolume = value; }
        }
    }
}
