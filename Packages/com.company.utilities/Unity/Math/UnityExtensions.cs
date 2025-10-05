namespace Company.Utilities.Unity
{
    public static class UnityExtensions
    {
        public static System.Numerics.Vector2 ToNumerics(this UnityEngine.Vector2 vector)
        {
            return new System.Numerics.Vector2(vector.x, vector.y);
        }

        public static UnityEngine.Vector2 To2D(this UnityEngine.Vector3 vector)
        {
            return new UnityEngine.Vector2(vector.x, vector.y);
        }

        public static System.Numerics.Vector3 ToNumerics(this UnityEngine.Vector3 vector)
        {
            return new System.Numerics.Vector3(vector.x, vector.y, vector.z);
        }
    }
}