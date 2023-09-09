using UnityEngine;

namespace JoyTeam
{
    public static class Math
    {
        private static bool CompareWithThreshold(float a, float b, float t)
        {
            return (a > (b - t)) && (a < (b + t));
        }

        public static bool CompareWithThreshold(Vector2 a, Vector2 b, float t)
        {
            return CompareWithThreshold(a.x, b.x, t) && CompareWithThreshold(a.y, b.y, t);
        }
    }
}
