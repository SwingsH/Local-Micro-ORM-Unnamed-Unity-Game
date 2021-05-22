using TIZSoft.Utils;
using UnityEngine;

namespace TIZSoft.Extensions
{
    /// <summary>
    /// 提供 <see cref="UnityEngine.Component"/> 擴充方法。
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// 取得或添加 Component。
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static TComponent GetOrAddComponent<TComponent>(this Component component)
            where TComponent : Component
        {
            ExceptionUtils.VerifyArgumentNull(component, "component");
            return component.gameObject.GetOrAddComponent<TComponent>();
        }

        /// <summary>
        /// 取得或添加 Component，並 assign 到 <paramref name="target"/>。
        /// </summary>
        /// <param name="component"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TComponent GetOrAddComponent<TComponent>(this Component component, ref TComponent target)
            where TComponent : Component
        {
            ExceptionUtils.VerifyArgumentNull(component, "component");
            return target = component.gameObject.GetOrAddComponent<TComponent>();
        }
    }
}
