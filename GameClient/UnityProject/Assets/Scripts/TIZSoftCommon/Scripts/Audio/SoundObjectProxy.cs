using UnityEngine;

namespace TIZSoft.Audio
{
    /// <summary>
    /// 用來避免使用端直接 dispose 真正的那個 SoundObject。
    /// 實際上在使用的 SoundObject 只是還給物件池，而這 Proxy 必須被摧毀不能再使用。
    /// </summary>
    class SoundObjectProxy : ISoundObject
    {
        ISoundObject actual;
        bool isReturned;

        internal SoundObjectProxy(ISoundObject actual)
        {
            this.actual = actual;
        }

        public void Dispose()
        {
            if (isReturned)
            {
                return;
            }

            isReturned = true;
            actual.Dispose();
            actual = null;
        }

        public ISoundObject SetClip(string clipName)
        {
            return actual.SetClip(clipName);
        }

        public ISoundObject SetClip(AudioClip clip)
        {
            return actual.SetClip(clip);
        }

        public ISoundObject SetLoop(bool isLoop)
        {
            return actual.SetLoop(isLoop);
        }

        public ISoundObject SetVolume(float volume)
        {
            return actual.SetVolume(volume);
        }

        public ISoundObject SetPosition(Vector3 worldPosition)
        {
            return actual.SetPosition(worldPosition);
        }
        
        public void Play()
        {
            actual.Play();
        }

        public void Pause()
        {
            actual.Pause();
        }

        public void Resume()
        {
            actual.Resume();
        }

        public void Stop()
        {
            actual.Stop();
        }

        public void FadingStop(float duration)
        {
            actual.FadingStop(duration);
        }

        public float GetTime()
        {
            return actual.GetTime();
        }
    }
}