using UniRx.Toolkit;
using UnityEngine;

namespace TIZSoft.Audio
{
    /// <summary>
    /// 表示一個 SoundObject 的物件池，用來回收再利用音效物件。
    /// </summary>
    class SoundObjectPool : ObjectPool<SoundObject>
    {
        readonly Transform hierarchyParent;
        readonly SoundObject prefab;
        readonly string prefixName;

        public SoundObjectPool(Transform hierarchyParent, SoundObject prefab, string prefixName)
        {
            this.hierarchyParent = hierarchyParent;
            this.prefab = prefab;
            this.prefixName = prefixName;
        }
        
        protected override SoundObject CreateInstance()
        {
            var instance = Object.Instantiate(prefab, hierarchyParent);
            const string clone = "(Clone)";
            var name = instance.name;
            instance.name = string.Concat(prefixName, name.Substring(0, name.Length - clone.Length));
            return instance;
        }

        protected override void OnBeforeReturn(SoundObject instance)
        {
            base.OnBeforeReturn(instance);

            instance.AudioSource.Stop();
            instance.AudioSource.clip = null;
            instance.transform.localPosition = Vector3.zero;
        }
    }
}
