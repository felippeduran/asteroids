using UnityEngine;

namespace Company.Utilities.Runtime
{
    public class Random
    {
        readonly System.Random random;

        public Random()
        {
            random = new System.Random();
        }

        public float Range(float min, float max)
        {
            return NextFloat() * (max - min) + min;
        }

        public int Range(int min, int max)
        {
            return random.Next(min, max);
        }

        public float NextFloat()
        {
            return (float)random.NextDouble();
        }

        public Vector2 NextDirection()
        {
            return (new Vector2((float)random.NextDouble(), (float)random.NextDouble()) - 0.5f * Vector2.one).normalized;
        }

        public Vector2 InsideUnitCircle()
        {
            return NextFloat() * NextDirection();
        }

        public Vector2 GetRandomDirectionFromCone(Vector2 linearVelocity, float angle)
        {
            var random = new Random();
            var randomAngle = random.Range(-angle / 2, angle / 2);
            var randomDirection = Quaternion.Euler(0, 0, randomAngle) * linearVelocity.normalized;
            return randomDirection;
        }
    }
}