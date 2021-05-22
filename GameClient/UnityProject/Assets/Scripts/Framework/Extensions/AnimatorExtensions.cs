using UnityEngine;
using System.Collections.Generic;

namespace TIZSoft.Extensions
{
    public static class AnimatorExtensions
    {
        public static List<AnimationClip> GetEventFiredClips(this Animator animator)
        {
            var result = new List<AnimationClip>();

            if (animator == null)
            {
                return result;
            }

            for (int i = 0; i < animator.layerCount; i++)
            {
                foreach (var clipInfo in animator.GetCurrentAnimatorClipInfo(i))
                {
                    if (IsEventFired(clipInfo))
                        result.Add(clipInfo.clip);
                }

                // 事件在transition當中被觸發時，CurrentState 可能不是觸發事件的 State
                if (animator.IsInTransition(i))
                {
                    foreach (var clipInfo in animator.GetNextAnimatorClipInfo(i))
                    {
                        if (IsEventFired(clipInfo))
                        {
                            result.Add(clipInfo.clip);
                        }
                    }
                }
            }
            return result;
        }

        static bool IsEventFired(AnimatorClipInfo clipInfo)
        {
            foreach (var clipEvent in clipInfo.clip.events)
            {
                if (clipEvent.isFiredByAnimator)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
