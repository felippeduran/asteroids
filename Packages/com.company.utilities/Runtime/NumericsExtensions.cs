using System;
using System.Numerics;

namespace Company.Utilities.Runtime
{
    public static class NumericsExtensions
    {
        public static float Magnitude(this Vector2 vector)
        {
            return Vector2.Distance(Vector2.Zero, vector);
        }

        public static float Angle(Vector2 vector, Vector2 other)
        {
            var angleInRadians = Math.Acos(Vector2.Dot(vector, other) / (vector.Magnitude() * other.Magnitude()));
            return (float)(angleInRadians * (180 / Math.PI));
        }
    }
}