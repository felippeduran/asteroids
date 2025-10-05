namespace Company.Utilities.Unity
{
    public static class NumericsExtensions
    {
        public static UnityEngine.Vector2 ToUnity(this System.Numerics.Vector2 vector)
        {
            return new UnityEngine.Vector2(vector.X, vector.Y);
        }

        public static System.Numerics.Vector3 To3D(this System.Numerics.Vector2 vector)
        {
            return new System.Numerics.Vector3(vector.X, vector.Y, 0);
        }

        public static UnityEngine.Vector3 ToUnity(this System.Numerics.Vector3 vector)
        {
            return new UnityEngine.Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}