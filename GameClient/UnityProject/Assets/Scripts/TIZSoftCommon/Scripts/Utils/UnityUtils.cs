using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TIZSoft.Utils
{
    using SceneManager = UnityEngine.SceneManagement.SceneManager;

    public static class UnityUtils
    {
        /// <summary>
        /// 取得所有存在於場景中的指定型別之物件。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeInactive">是否要包括 inactive 的物件</param>
        /// <param name="sceneFilter">場景篩選器</param>
        /// <returns></returns>
        public static T[] FindObjectsOfType<T>(bool includeInactive = false, Func<Scene, bool> sceneFilter = null)
            where T : Object
        {
            IList<T> output = new List<T>();
            FindObjectsOfType(ref output, includeInactive, sceneFilter);
            return output.ToArray();
        }

        public static void FindObjectsOfType<T>(
            ref IList<T> output,
            bool includeInactive = false,
            Func<Scene, bool> sceneFilter = null)
            where T : Object
        {
            if (output == null)
            {
                output = new List<T>();
            }

            for (var i = 0; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (sceneFilter == null || sceneFilter(scene))
                {
                    foreach (var go in scene.GetRootGameObjects())
                    foreach (var component in go.GetComponentsInChildren<T>(includeInactive))
                    {
                        output.Add(component);
                    }
                }
            }
        }
    }
}
