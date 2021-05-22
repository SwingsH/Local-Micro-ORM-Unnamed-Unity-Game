using TIZSoft.Utils;
using UnityEngine;

namespace TIZSoft.Extensions
{
    /// <summary>
    /// 提供 <see cref="UnityEngine.GameObject"/> 擴充方法。
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 取得或添加 Component。
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject)
            where TComponent : Component
        {
            ExceptionUtils.VerifyArgumentNull(gameObject, "gameObject");
            var component = gameObject.GetComponent<TComponent>();
            return component ? component : gameObject.AddComponent<TComponent>();
        }

        /// <summary>
        /// 取得或添加 Component，並 assign 到 <paramref name="target"/>。
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject, ref TComponent target)
            where TComponent : Component
        {
            ExceptionUtils.VerifyArgumentNull(gameObject, "gameObject");
            return target = gameObject.GetOrAddComponent<TComponent>(); ;
        }

        public static void AttachLocalIdentity(this GameObject gameObject, Transform parent)
        {
            ExceptionUtils.VerifyArgumentNull(gameObject, "gameObject");
            gameObject.transform.AttachLocalIdentity(parent);
        }

        public static void AttachLocal(this GameObject gameObject,
                                       Transform parent,
                                       Vector3 localPosition,
                                       Quaternion localRotation,
                                       Vector3 localScale)
        {
            ExceptionUtils.VerifyArgumentNull(gameObject, "gameObject");
            gameObject.transform.AttachLocal(parent, localPosition, localRotation, localScale);
        }

        /// <summary>
        /// 雖然可以動態使用，不過會因為旋轉或動畫有不一樣的大小，所以還是建議編輯時使用
        /// </summary>
        public static TCollider AttachCollider<TCollider>(this GameObject gameObject)
            where TCollider : Collider
        {
            ExceptionUtils.VerifyArgumentNull(gameObject, "gameObject");
            var collider = gameObject.GetOrAddComponent<TCollider>();

            var bound = new Bounds
            {
                min = gameObject.transform.position,
                max = gameObject.transform.position
            };

            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (renderer is SkinnedMeshRenderer)
                {
                    if (!(renderer as SkinnedMeshRenderer).sharedMesh)
                        continue;
                }
                bound.min = Vector3.Min(renderer.bounds.min, bound.min);
                bound.max = Vector3.Max(renderer.bounds.max, bound.max);
            }
            bound.min = gameObject.transform.InverseTransformPoint(bound.min);
            bound.max = gameObject.transform.InverseTransformPoint(bound.max);

            if (collider is BoxCollider)
            {
                var boxCollider = collider as BoxCollider;
                boxCollider.center = bound.center;
                boxCollider.size = bound.size;
            }
            else if (collider is SphereCollider)
            {
                var sphereCollider = collider as SphereCollider;
                sphereCollider.center = bound.center;
                sphereCollider.radius = Mathf.Max(bound.extents.x, bound.extents.y, bound.extents.z);
            }
            else if (collider is CapsuleCollider)
            {
                var capsuleCollider = collider as CapsuleCollider;
                capsuleCollider.center = bound.center;
                capsuleCollider.height = bound.size.y;
                capsuleCollider.radius = Mathf.Max(bound.extents.x, bound.extents.z);
            }
            return collider;
        }

        /// <summary>
        /// 取得此物件所屬Child子節點Compnent Nick 20171012
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RootObj">根節點物件</param>
        /// <param name="sFindName">搜尋的物件名稱</param>
        /// <returns></returns>
        public static T GetObjCompnent<T>(GameObject RootObj, string sFindName)
        {
            if (RootObj == null)
                return default(T);

            if (string.IsNullOrEmpty(sFindName))
                return default(T);

            Transform GetTrans = RootObj.transform;
            if (GetTrans == null)
                return default(T);

            if (RootObj.name == sFindName)
            {
                T GetCom = GetTrans.GetComponent<T>();
                if (GetCom != null)
                    return GetCom;
            }

            Transform GetChildTrans = FindDeepChild(GetTrans, sFindName);
            if (GetChildTrans == null)
                return default(T);

            GameObject GetObj = GetChildTrans.gameObject;
            if (GetObj == null)
                return default(T);

            return GetObj.GetComponent<T>();
        }


        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string sFindName)
        {
            var result = aParent.Find(sFindName);
            if (result != null)
                return result;

            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(sFindName);
                if (result != null)
                    return result;
            }
            return null;
        }


        /*
        //Depth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            foreach(Transform child in aParent)
            {
                if(child.name == aName )
                    return child;
                var result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }
        */
    }
    public static class ParticleExtensiongs
    {
        public static void SetParticleSortingOrder(this GameObject gameObject)
        {
            if (!gameObject)
                return;
            var canvas = gameObject.GetComponentInParent<Canvas>();
            var sortingOrder = 0;
            if (canvas)
                sortingOrder = canvas.sortingOrder;
            SetParticleSortingOrder(gameObject, sortingOrder);
        }
        public static void SetParticleSortingOrder(this GameObject gameObject, int sortingOrder)
        {
            if (!gameObject)
                return;
            foreach (var psr in gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true))
            {
                psr.sortingOrder = sortingOrder;
            }
        }

        public static void SetParticleSortingOrder(this Component component)
        {
            if (!component)
                return;
            SetParticleSortingOrder(component.gameObject);
        }
        public static void SetParticleSortingOrder(this Component component, int sortingOrder)
        {
            if (!component)
                return;
            SetParticleSortingOrder(component.gameObject, sortingOrder);
        }
    }
    public static class RectTransformExtensions
    {
        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = new Vector3(1, 1, 1);
        }
        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }
        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }
        public static float GetWidth(this RectTransform trans)
        {
            return trans.rect.width;
        }
        public static float GetHeight(this RectTransform trans)
        {
            return trans.rect.height;
        }
        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }
        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }
        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }
        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }
        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }
        public static void SetWidth(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(newSize, trans.rect.size.y));
        }
        public static void SetHeight(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(trans.rect.size.x, newSize));
        }
    }
}
