using System;
using System.Collections;
//using DG.Tweening;
using TIZSoft.Log;
using UniRx;
using UnityEngine;
using Logger = TIZSoft.Log.Logger;

namespace TIZSoft.Audio
{
    class SoundObject : MonoBehaviour, ISoundObject
    {
        enum State
        {
            Playing,
            Paused,
            Stopped
        }
        
        static readonly Logger logger = LogManager.Default.FindOrCreateCurrentTypeLogger();

        [SerializeField]
        [Tooltip("音源")]
        AudioSource audioSource;

        public AudioSource AudioSource { get { return audioSource; } }

        SoundManager soundManager;
        AudioListener audioListener;
        AssetProvider assetProvider;
        string assetCategory;
        SoundObjectPool soundObjectPool;

        State state = State.Paused;

        AudioMode audioMode;
        float volume = 1F;
        string clipName;
        AudioClip lastClip;
        AudioClip clip;

        IDisposable soundEffectVolumeListener;
        IDisposable backgroundMusicVolumeListener;

        public SoundObject SetSoundManager(SoundManager soundManager)
        {
            this.soundManager = soundManager;

            if (soundEffectVolumeListener == null)
            {
                soundEffectVolumeListener = soundManager.SoundEffectVolume.Subscribe(_ => UpdateVolume());
            }

            if (backgroundMusicVolumeListener == null)
            {
                backgroundMusicVolumeListener = soundManager.BackgroundMusicVolume.Subscribe(_ => UpdateVolume());
            }

            return this;
        }
        
        void UpdateVolume()
        {
            if (AudioSource == null)
            {
                return;
            }

            switch (audioMode)
            {
                case AudioMode.SoundEffect:
                    AudioSource.volume = volume*soundManager.SoundEffectVolume.Value;
                    break;
                case AudioMode.BackgroundMusic:
                    AudioSource.volume = volume*soundManager.BackgroundMusicVolume.Value;
                    break;
            }
        }

        public SoundObject SetAudioListener(AudioListener audioListener)
        {
            this.audioListener = audioListener;
            return this;
        }

        public SoundObject SetAssetProvider(AssetProvider assetProvider)
        {
            this.assetProvider = assetProvider;
            return this;
        }

        public SoundObject SetAssetCategory(string assetCategory)
        {
            this.assetCategory = assetCategory;
            return this;
        }

        public SoundObject SetPool(SoundObjectPool soundObjectPool)
        {
            this.soundObjectPool = soundObjectPool;
            return this;
        }

        public ISoundObject SetClip(string clipName)
        {
            this.clipName = clipName;
            return this;
        }

        public ISoundObject SetClip(AudioClip clip)
        {
            lastClip = this.clip;
            this.clip = clip;
            return this;
        }

        public ISoundObject SetLoop(bool isLoop)
        {
            AudioSource.loop = isLoop;
            return this;
        }

        public ISoundObject SetVolume(float volume)
        {
            this.volume = volume;
            UpdateVolume();
            return this;
        }
        
        public ISoundObject SetPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
            return this;
        }

        public SoundObject SetAudioMode(AudioMode audioMode)
        {
            this.audioMode = audioMode;
            return this;
        }

        public SoundObject AsSoundEffect()
        {
            audioMode = AudioMode.SoundEffect;
            return this;
        }

        public SoundObject AsBackgroundMusic()
        {
            audioMode = AudioMode.BackgroundMusic;
            return this;
        }

        public void Play()
        {
            state = State.Playing;

            if (lastClip == clip &&
                AudioSource.clip != null)
            {
                PlayInternal();
            }
            else if (!string.IsNullOrEmpty(clipName))
            {
                new SoundObjectLoader(this).Subscribe(_ => PlayInternal());
            }
            else
            {
                logger.Error("No AudioClip or clip name.");
                state = State.Stopped;
            }
        }

        public void Pause()
        {
            if (state == State.Playing)
            {
                state = State.Paused;
                AudioSource.Pause();
            }
        }

        public void Resume()
        {
            if (state == State.Paused)
            {
                state = State.Playing;
                AudioSource.UnPause();
            }
        }

        public void Stop()
        {
            state = State.Stopped;
            AudioSource.Stop();
            AudioSource.clip = null;
        }

        public void FadingStop(float duration)
        {
            state = State.Stopped;
            //AudioSource.DOFade(0F, duration)
            //    .OnComplete(Stop)
            //    .Play();
        }

        void PlayInternal()
        {
            Observable.FromMicroCoroutine(PlayAndWaitUntilFinished).Subscribe();
        }
        
        void IDisposable.Dispose()
        {
            Return();
        }
        
        public void Return()
        {
            if (audioMode == AudioMode.SoundEffect)
            {
                soundObjectPool.Return(this);
            }
        }

        IEnumerator PlayAndWaitUntilFinished()
        {
            UpdateVolume();

            if (audioMode == AudioMode.BackgroundMusic)
            {
                AudioSource.loop = true;
                transform.position = audioListener.transform.position;

                const float fadeInDuration = 1F;
                //AudioSource.DOFade(0F, fadeInDuration).Play(); // shmarked
                var fadeStartTime = Time.time;
                while (Time.time - fadeStartTime < fadeInDuration)
                {
                    yield return null;
                }
            }

            AudioSource.clip = clip;
            AudioSource.Play();

            if (audioMode == AudioMode.BackgroundMusic)
            {
                const float fadeOutDuration = 1F;
                AudioSource.volume = 1F;
                //AudioSource.DOFade(0F, fadeOutDuration).From().Play();
                var fadeStartTime = Time.time;
                while (Time.time - fadeStartTime < fadeOutDuration)
                {
                    yield return null;
                }
            }

            if (audioMode == AudioMode.SoundEffect && !AudioSource.loop)
            {
                // Unity 沒提供任何 callback 或 event 可得知 Audio 是否已播完，要自己處理。
                var startTime = Time.time;
                while (AudioSource.clip != null &&
                       Time.time - startTime < AudioSource.clip.length)
                {
                    yield return null;
                }

                Return();
            }
        }

        public float GetTime()
        {
            return AudioSource.time;
        }

        class SoundObjectLoader : IObservable<ISoundObject>
        {
            readonly SoundObject parent;

            public SoundObjectLoader(SoundObject parent)
            {
                this.parent = parent;
            }

            public IDisposable Subscribe(IObserver<ISoundObject> observer)
            {
                parent.assetProvider.LoadAsync<AudioClip>(
                    parent.assetCategory,
                    parent.clipName,
                    clip =>
                    {
                        parent.clip = clip;
                        observer.OnNext(parent);
                        observer.OnCompleted();
                    });
                return Disposable.Empty;
            }
        }
    }
}
