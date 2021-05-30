#define Q9994 // 釋放BGM記憶體 Nick 20180612

using System;
using TIZSoft.SaveData;
using UniRx;
using UnityEngine;

namespace TIZSoft.Audio
{
    /// <summary>
    /// 表示一個 Sound 管理器，負責管理、播放音效。
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code>
    /// soundManager.GetSfxObject()
    ///     .SetClip("clip_name")
    ///     .SetLoop(false)
    ///     .SetPosition(targetTransform.position)
    ///     .Play();
    /// </code>
    /// </example>
    /// </remarks>
    [AddComponentMenu("TIZSoft/Sound Manager")]
    public class SoundManager : MonoBehaviour
    {
        AssetProvider assetProvider;
        AssetProvider streamingAssetProvider;
        SoundObjectPool sfxObjectPool;
        SoundObjectPool bgmObjectPool;

        [SerializeField]
        AudioListener audioListener;

        [SerializeField]
        [Tooltip("SFx AssetBundle Category.")]
        string assetCategory = "se";

        [SerializeField]
        [Tooltip("BGM AssetBundle Category.")]
        string backgroundMusicAssetCategory = "bgm";

        [SerializeField]
        [Tooltip("SoundObject prefab.")]
        SoundObject soundObjectPrefab;
        
        readonly BoolReactiveProperty isInitialized = new BoolReactiveProperty();

        [SerializeField]
        [Tooltip("總音量。")]
        FloatReactiveProperty audioVolume = new FloatReactiveProperty(1F);
        
        [SerializeField]
        [Tooltip("音效音量。")]
        FloatReactiveProperty soundEffectVolume = new FloatReactiveProperty(1F);
        
        [SerializeField]
        [Tooltip("背景音樂音量。")]
        FloatReactiveProperty backgroundMusicVolume = new FloatReactiveProperty(1F);

        SoundObject bgmObject;

        public IReadOnlyReactiveProperty<bool> IsInitialized
        {
            get { return isInitialized.ToReadOnlyReactiveProperty(); }
        }

        /// <summary>
        /// 取得或設定總音訊音量。
        /// </summary>
        public ReactiveProperty<float> AudioVolume
        {
            get { return audioVolume; }
        }

        /// <summary>
        /// 取得或設定音效 (SFx) 音量。
        /// </summary>
        public ReactiveProperty<float> SoundEffectVolume
        {
            get { return soundEffectVolume; }
        }

        /// <summary>
        /// 取得或設定背景音樂 (BGM) 音量。
        /// </summary>
        public ReactiveProperty<float> BackgroundMusicVolume
        {
            get { return backgroundMusicVolume; }
        }
        
        public void Initialize(AssetProvider assetProvider, AssetProvider streamingAssetProvider, SystemSave systemSave)
        {
            if (isInitialized.Value)
            {
                return;
            }

            this.assetProvider = assetProvider;
            this.streamingAssetProvider = streamingAssetProvider;
            sfxObjectPool = new SoundObjectPool(transform, soundObjectPrefab, "[SFx]");
            bgmObjectPool = new SoundObjectPool(transform, soundObjectPrefab, "[BGM]");

            audioVolume.Value = systemSave.AudioVolume;
            soundEffectVolume.Value = systemSave.SfxVolume;
            backgroundMusicVolume.Value = systemSave.BgmVolume;
            AudioListener.volume = audioVolume.Value;

            soundEffectVolume.Subscribe(v => systemSave.SfxVolume = v);
            backgroundMusicVolume.Subscribe(v => systemSave.BgmVolume = v);

            isInitialized.SetValueAndForceNotify(true);
        }
        
        /// <summary>
        /// 取得一個可播放音效的物件。
        /// </summary>
        /// <returns></returns>
        public ISoundObject GetSfxObject()
        {
            CheckInitializationState();

            var soundObject = sfxObjectPool.Rent()
                .SetSoundManager(this)
                .SetAudioListener(audioListener)
                .SetAssetProvider(assetProvider.IsInitialized.Value ? assetProvider : streamingAssetProvider)
                .SetAssetCategory(assetCategory)
                .SetPool(sfxObjectPool)
                .AsSoundEffect();
            return new SoundObjectProxy(soundObject);
        }

        /// <summary>
        /// 取得一個可播放 BGM 的物件。
        /// </summary>
        /// <returns></returns>
        public ISoundObject GetBgmObject()
        {
            CheckInitializationState();

            if (bgmObject == null)
            {
                bgmObject = bgmObjectPool
                    .Rent()
                    .SetSoundManager(this)
                    .SetAudioListener(audioListener)
                    .SetAssetProvider(assetProvider.IsInitialized.Value ? assetProvider : streamingAssetProvider)
                    .SetAssetCategory(backgroundMusicAssetCategory)
                    .SetPool(bgmObjectPool)
                    .AsBackgroundMusic();
            }
            return new SoundObjectProxy(bgmObject);
        }

        void CheckInitializationState()
        {
            if (IsInitialized.Value)
            {
                return;
            }

            throw new InvalidOperationException("SoundManager not been initialized.");
        }

#if Q9994 // 釋放BGM記憶體 Nick 20180612
        public void ReturnBgmObject()
        {
            if (bgmObjectPool == null)
            {
                bgmObject.ClearClip();
                bgmObject = null;
                return;
            }

            if (bgmObject != null)
                bgmObjectPool.Return(bgmObject);

            bgmObject.ClearClip();
            bgmObject = null;
        }

        public void ClearBgmPool()
        {
            if (bgmObjectPool == null)
                return;

            bgmObjectPool.Clear();
        }
#endif //Q9994
    }
}
