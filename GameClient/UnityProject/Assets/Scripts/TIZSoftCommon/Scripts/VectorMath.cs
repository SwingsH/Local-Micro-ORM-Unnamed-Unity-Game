using UnityEngine;

namespace TIZSoft
{
    /// <summary>
    /// 提供一些向量計算方法。
    /// </summary>
    public static class VectorMath
    {
        /// <summary>
        /// Evaluates the z-component of cross product of vectors.
        /// </summary>
        /// <param name="v1">First vector.</param>
        /// <param name="v2">Second vector.</param>
        /// <returns>z-component of cross product.</returns>
        /// <remarks>
        /// See http://allenchou.net/2013/07/cross-product-of-2d-vectors/
        /// </remarks>
        public static float Cross(this Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }
    }
}
