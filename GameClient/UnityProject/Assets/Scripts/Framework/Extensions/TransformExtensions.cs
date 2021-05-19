using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tizsoft.Extensions
{
    /// <summary>
    /// 定義 Transform 搜尋選項。
    /// </summary>
    public enum TransformSearchOption
    {
        /// <summary>
        /// 只搜尋第一層。
        /// </summary>
        TopOnly,

        /// <summary>
        /// 搜尋所有子階層。
        /// </summary>
        All
    }

    /// <summary>
    /// 定義 Transform 走訪順序。
    /// </summary>
    public enum TraversalOption
    {
        /// <summary>
        /// Breadth-first search. 廣度優先走訪。
        /// </summary>
        Bfs,

        /// <summary>
        /// Depth-first search. 深度優先走訪。
        /// </summary>
        Dfs
    }

    /// <summary>
    /// Unity <see cref="Transform"/> 擴充方法。
    /// </summary>
    /// <remarks>
    /// TODO: 回傳陣列可考慮使用 <see cref="IList{T}"/>，避免呼叫 <see cref="List{T}.ToArray"/> 浪費效率。
    /// </remarks>
    public static class TransformExtensions
    {
        [Obsolete("Use 'transform.root' instead.")]
        public static Transform FindRoot(this Transform transform)
        {
            return transform == null ? null : transform.root;
        }

        /// <summary>
        /// 取得所有子 <see cref="Transform"/>，此方法將會搜尋所有子階層，並採用廣度優先。
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform[] GetChildren(this Transform transform)
        {
            return transform.GetChildren(TransformSearchOption.All, TraversalOption.Bfs, null);
        }

        /// <summary>
        /// 取得所有子 <see cref="Transform"/>，此方法將會採用廣度優先。
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static Transform[] GetChildren(this Transform transform, TransformSearchOption searchOption)
        {
            return transform.GetChildren(searchOption, TraversalOption.Bfs, null);
        }

        /// <summary>
        /// 取得所有子 <see cref="Transform"/>，此方法將會搜尋所有子階層。
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="traversalOption"></param>
        /// <returns></returns>
        public static Transform[] GetChildren(this Transform transform, TraversalOption traversalOption)
        {
            return transform.GetChildren(TransformSearchOption.All, traversalOption, null);
        }

        /// <summary>
        /// 依條件篩選取得所有子 <see cref="Transform"/>，此方法將會搜尋所有子階層，並採用廣度優先。
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="predicate">條件。</param>
        /// <returns></returns>
        public static Transform[] GetChildren(this Transform transform, Predicate<Transform> predicate)
        {
            return transform.GetChildren(TransformSearchOption.All, TraversalOption.Bfs, predicate);
        }

        /// <summary>
        /// 根據搜尋設定取得所有符合條件的子 <see cref="Transform"/>。
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="searchOption"></param>
        /// <param name="traversalOption"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Transform[] GetChildren(this Transform transform,
            TransformSearchOption searchOption,
            TraversalOption traversalOption,
            Predicate<Transform> predicate)
        {
            switch (traversalOption)
            {
                case TraversalOption.Bfs:
                    return Bfs(transform, searchOption, predicate);

                default:
                    return Dfs(transform, searchOption, predicate);
            }
        }

        static Transform[] Bfs(this Transform transform,
            TransformSearchOption searchOption,
            Predicate<Transform> predicate)
        {
            if (transform == null)
            {
                return new Transform[0];
            }

            if (transform.childCount == 0)
            {
                return new Transform[0];
            }

            var discovered = new HashSet<Transform>();
            var noPredicate = predicate == null;
            var matches = new List<Transform>();
            var traversal = new Queue<Transform>();

            traversal.Enqueue(transform);
            while (traversal.Count != 0)
            {
                var current = traversal.Dequeue();
                if (discovered.Contains(current))
                {
                    continue;
                }

                discovered.Add(current);
                for (var index = 0; index != current.childCount; ++index)
                {
                    var child = current.GetChild(index);
                    if (searchOption == TransformSearchOption.All)
                    {
                        traversal.Enqueue(child);
                    }

                    if (noPredicate || predicate(child))
                    {
                        matches.Add(child);
                    }
                }
            }

            return matches.ToArray();
        }

        static Transform[] Dfs(this Transform transform,
            TransformSearchOption searchOption,
            Predicate<Transform> predicate)
        {
            if (transform == null)
            {
                return new Transform[0];
            }

            if (transform.childCount == 0)
            {
                return new Transform[0];
            }

            var discovered = new HashSet<Transform>();
            var noPredicate = predicate == null;
            var matches = new List<Transform>();
            var traversal = new Stack<Transform>();

            traversal.Push(transform);
            while (traversal.Count != 0)
            {
                var current = traversal.Pop();
                if (discovered.Contains(current))
                {
                    continue;
                }

                discovered.Add(current);
                for (var index = 0; index != current.childCount; ++index)
                {
                    var child = current.GetChild(index);
                    if (searchOption == TransformSearchOption.All)
                    {
                        traversal.Push(child);
                    }

                    if (noPredicate || predicate(child))
                    {
                        matches.Add(child);
                    }
                }
            }

            return matches.ToArray();
        }

        public static TComponent GetOrAddComponent<TComponent>(this Transform transform)
            where TComponent : Component
        {
            if (transform == null)
            {
                return null;
            }

            return transform.gameObject.GetOrAddComponent<TComponent>();
        }

        public static void AttachLocalIdentity(this Transform transform, Transform parent)
        {
            AttachLocal(transform, parent, Vector3.zero, Quaternion.identity, Vector3.one);
        }

        public static void AttachLocal(this Transform transform,
            Transform parent,
            Vector3 localPosition,
            Vector3 localEulerAngles,
            Vector3 localScale)
        {
            if (transform == null)
            {
                return;
            }

            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localEulerAngles = localEulerAngles;
            transform.localScale = localScale;
        }

        public static void AttachLocal(this Transform transform,
            Transform parent,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale)
        {
            if (transform == null)
            {
                return;
            }

            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = localScale;
        }

        /// <summary>
        /// Gets transforms from parent to root.
        /// </summary>
        /// <returns></returns>
        public static Transform[] GetAncestors(this Transform transform)
        {
            if (transform == null)
            {
                return new Transform[0];
            }

            if (transform.parent == null)
            {
                return new Transform[0];
            }

            var current = transform.parent;
            var ancestors = new List<Transform>();

            while (current != null)
            {
                ancestors.Add(current);
                current = current.parent;
            }

            return ancestors.ToArray();
        }
    }
}
