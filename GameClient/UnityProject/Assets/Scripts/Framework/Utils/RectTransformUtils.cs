using UnityEngine;

namespace TIZSoft.Utils
{
	public static class RectTransformUtils
	{
        /// <summary>
        /// 將世界座標點轉成 Screen Space 上的 Viewport Point。
        /// 計算出來的 Viewport Point 將會是以 Canvas Render Mode 為 Screen Space - Overlay 基準的座標點。
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public static Vector2 WorldToViewportPoint(Vector2 worldPoint)
		{
			return WorldToViewportPoint(worldPoint, null);
		}

        /// <summary>
        /// 將世界座標點轉成 Screen Space 上的 Viewport Point。
        /// 如果 Canvas Render Mode 是 Screen Space - Overlay，可以不用給定 camera。
        /// </summary>
        /// <param name="camera">可以是</param>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public static Vector2 WorldToViewportPoint(Vector2 worldPoint, Camera camera)
		{
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPoint);
            var viewportPoint = new Vector2(screenPoint.x/Screen.width, screenPoint.y/Screen.height);
			return viewportPoint;
		}
	}
}
