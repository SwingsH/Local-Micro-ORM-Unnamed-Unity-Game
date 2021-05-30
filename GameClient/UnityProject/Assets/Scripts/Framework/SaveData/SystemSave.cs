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
        
        [SerializeField]
        [Tooltip("背景音樂開關。")]
        bool bgmValid = true;
        
        [SerializeField]
        [Tooltip("音效開關。")]
        bool seValid = true;
        
        [SerializeField]
        [Tooltip("語音開關。")]
        bool voiceValid = true;

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

        /// <summary>
        /// 取得或設定背景音樂是否有效。
        /// </summary>
        public bool BGMValid
        {
            get { return bgmValid; }
            set { bgmValid = value; }
        }

        /// <summary>
        /// 取得或設定音效 (SFx) 是否有效。
        /// </summary>
        public bool SoundEffectValid
        {
            get { return seValid; }
            set { seValid = value; }
        }

        /// <summary>
        /// 取得或設定語音是否有效。
        /// </summary>
        public bool VoiceValid
        {
            get { return voiceValid; }
            set { voiceValid = value; }
        }
    }
}
