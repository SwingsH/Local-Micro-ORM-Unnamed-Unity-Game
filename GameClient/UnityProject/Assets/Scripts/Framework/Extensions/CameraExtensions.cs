using UnityEngine;

namespace TIZSoft.Extensions
{
    public static class CameraExtensions
    {
        public static Vector3 RaycastScreenPointToWorld(this Camera camera, Vector3 screenPoint)
        {
            return RaycastScreenPointToWorld(camera, screenPoint, 100F);
        }

        public static Vector3 RaycastScreenPointToWorld(this Camera camera, Vector3 screenPoint, float raycastDistance)
        {
            if (camera == null)
            {
                return screenPoint;
            }

            var camRay = camera.ScreenPointToRay(screenPoint);
            RaycastHit hit;
            return Physics.Raycast(camRay, out hit, raycastDistance) ? hit.point : screenPoint;
        }

        /// <summary>
        /// 將攝影機座標移動至可拍攝目標物件的位置
        /// </summary>
        /// <param name="camera">指定攝影機</param>
        /// <param name="gameobject">目標物件</param>
        public static void FocusRenderObject_AlignTop(this Camera camera, GameObject gameobject)
        {
            // 計算目標物件的render位置大小
            var bound = new Bounds(gameobject.transform.position, Vector3.zero);
            foreach (var renderer in gameobject.GetComponentsInChildren<Renderer>())
            {
                bound.SetMinMax(
                    Vector3.Min(bound.min, renderer.bounds.min),
                    Vector3.Max(bound.max, renderer.bounds.max)
                );
            }

            camera.transform.position = gameobject.transform.position
                - camera.transform.forward * (camera.nearClipPlane + bound.extents.magnitude);


            var topTargetPosition = bound.center + Vector3.Project(bound.extents, camera.transform.up);
            var topTargetScreenPosition = camera.WorldToScreenPoint(topTargetPosition);
            var topPosition = camera.ScreenToWorldPoint(new Vector3(
                camera.pixelWidth * 0.5F,
                camera.pixelHeight,
                Vector3.Distance(camera.transform.position, bound.center)
            ));
            var delta = camera.ScreenToWorldPoint(topTargetScreenPosition) - topPosition;
            camera.transform.Translate(delta);
        }

        /// <summary>
        /// 將攝影機座標移動至可拍攝目標物件的位置
        /// </summary>
        /// <param name="camera">指定攝影機</param>
        /// <param name="targetRootGameObject">目標物件</param>
        /// <param name="alignTransofrm">指定對齊Transform</param>
        public static void FocusRenderObject_AlignCenter(this Camera camera,
                                                         GameObject targetRootGameObject,
                                                         Transform alignTransofrm)
        {
            // 計算目標物件的render位置大小
            var bound = new Bounds(targetRootGameObject.transform.position, Vector3.zero);
            foreach (var renderer in targetRootGameObject.GetComponentsInChildren<Renderer>())
            {
                bound.SetMinMax(
                    Vector3.Min(bound.min, renderer.bounds.min),
                    Vector3.Max(bound.max, renderer.bounds.max)
                );
            }

            camera.transform.position = alignTransofrm.position
                - camera.transform.forward * (camera.nearClipPlane + bound.extents.magnitude);
        }
    }
}
