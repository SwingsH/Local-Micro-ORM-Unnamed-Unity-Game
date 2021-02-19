using System;
using System.Collections.Generic;
using UnityEngine;

namespace TIZSoft.Extensions
{
    public enum TransformSearchOption
    {
        TopOnly,
        All
    }

    public enum TraversalOption
    {
        /// <summary>
        /// Breadth-first search.
        /// </summary>
        Bfs,

        /// <summary>
        /// Depth-first search.
        /// </summary>
        Dfs
    }

    public static class TransformExtensions
    {
        public static Transform FindRoot(this Transform transform)
        {
            return transform == null ? null : transform.root;
        }

        public static Transform[] GetChildren(this Transform transform)
        {
            return transform.GetChildren(TransformSearchOption.All, TraversalOption.Bfs, null);
        }

        public static Transform[] GetChildren(this Transform transform, TransformSearchOption searchOption)
        {
            return transform.GetChildren(searchOption, TraversalOption.Bfs, null);
        }

        public static Transform[] GetChildren(this Transform transform, TraversalOption traversalOption)
        {
            return transform.GetChildren(TransformSearchOption.All, traversalOption, null);
        }

        public static Transform[] GetChildren(this Transform transform, Predicate<Transform> predicate)
        {
            return transform.GetChildren(TransformSearchOption.All, TraversalOption.Bfs, predicate);
        }

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
