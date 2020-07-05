using UnityEngine;

namespace Utils
{
    public static class Maths
    {
        public static Vector3 Up(this Quaternion rotation)
        {
            return new Vector3
            {
                x = rotation.x * (rotation.y * 2f) - rotation.w * (rotation.z * 2f),
                y = 1.0f - (rotation.x * (rotation.x * 2f) + rotation.z * (rotation.z * 2f)),
                z = rotation.y * (rotation.z * 2f) + rotation.w * (rotation.x * 2f)
            };
        }

        public static float Dot(this Vector2 vec, Vector2 other)
        {
            return Vector2.Dot(vec, other);
        }
        public static float Dot(this Vector3 vec, Vector3 other)
        {
            return Vector3.Dot(vec, other);
        }
    }
}