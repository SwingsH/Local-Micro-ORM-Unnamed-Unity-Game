using System;
using UnityEngine;

namespace TIZSoft.Audio
{
    /// <summary>
    /// 表示一個音效播放物件。
    /// </summary>
    public interface ISoundObject : IDisposable
    {
        /// <summary>
        /// 設定 Audio clip 名稱，此設定方法會透過 AssetBundle 取得 AudioClip。
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns></returns>
        ISoundObject SetClip(string clipName);

        /// <summary>
        /// 設定 AudioClip。
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        ISoundObject SetClip(AudioClip clip);

        ISoundObject SetFinishedEvent(Action onFinished);

        /// <summary>
        /// 設定是否要循環播放。如果播放類型是 BGM，則此設定無效。
        /// </summary>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        ISoundObject SetLoop(bool isLoop, float loop_begin_time = 0, float loop_end_time = 0);

        /// <summary>
        /// 設定音量大小。
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        ISoundObject SetVolume(float volume);

        /// <summary>
        /// 設定要播放音效的位置。如果播放類型是 BGM，則此設定無效。
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        ISoundObject SetPosition(Vector3 worldPosition);

        /// <summary>
        /// 開始播放音訊。
        /// </summary>
        void Play();

        void Play(Action onFinished);

        /// <summary>
        /// 暫停播放。
        /// </summary>
        void Pause();

        /// <summary>
        /// 繼續播放。
        /// </summary>
        void Resume();

        /// <summary>
        /// 停止播放音訊。
        /// </summary>
        void Stop();

        /// <summary>
        /// 以淡出的方式停止播放音訊。
        /// </summary>
        void FadingStop(float duration);

        /// <summary>
        /// 取得目前播放時間點
        /// </summary>
        float GetTime();

        void ClearClip();
    }
}
