using UnityEngine;

namespace TIZSoft.Extensions
{
    /// <summary>
    /// Unity <see cref="Vector3"/> 擴充方法。
    /// </summary>
    public static class VectorExtensions
    {
        public static Vector2 SetX(this Vector2 vector, float value)
        {
            return new Vector2(value, vector.y);
        }

        public static Vector2 SetY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, value);
        }

        public static Vector2 AddX(this Vector2 vector, float value)
        {
            return new Vector2(vector.x + value, vector.y);
        }

        public static Vector2 AddY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, vector.y + value);
        }

        public static Vector3 SetX(this Vector3 vector, float value)
        {
            return new Vector3(value, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, value, vector.z);
        }

        public static Vector3 SetZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, value);
        }

        public static Vector3 AddX(this Vector3 vector, float value)
        {
            return new Vector3(vector.x + value, vector.y, vector.z);
        }

        public static Vector3 AddY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y + value, vector.z);
        }

        public static Vector3 AddZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, vector.z + value);
        }

        public static Vector3 PositionFromHere(this Vector3 position, Vector3 direction, float distance)
        {
            var moveDir = new Ray(position, direction);
            return moveDir.GetPoint(distance);
        }

        public static Vector3 ScreenPointToWorld(this Camera camera, Vector3 point, float castDistance = float.PositiveInfinity)
        {
            var cam = camera ?? Camera.main;
            RaycastHit hit;
            return Physics.Raycast(cam.ScreenPointToRay(point), out hit, castDistance) ? hit.point : point;
        }
    }
}
