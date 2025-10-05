using System;
using System.Numerics;

namespace Gameplay.Simulation.Runtime
{
    [Serializable]
    public struct Bounds
    {
        public SerializableVector2 Center;
        public SerializableVector2 Extents;
        public Vector2 Min => (Vector2)Center - Extents;
        public Vector2 Max => (Vector2)Center + Extents;
        public Vector2 Size => (Vector2)Extents * 2f;

        public Bounds(Vector2 center, Vector2 size)
        {
            this.Center = center;
            this.Extents = size * 0.5f;
        }

        public bool Contains(Vector2 point)
        {
            return point.X >= Min.X && point.X <= Max.X && point.Y >= Min.Y && point.Y <= Max.Y;
        }

        public override string ToString()
        {
            return $"Min: {Min}, Max: {Max}";
        }
    }

    [Serializable]
    public struct SerializableVector2
    {
        public float X;
        public float Y;

        public static implicit operator Vector2(SerializableVector2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static implicit operator SerializableVector2(Vector2 vector)
        {
            return new SerializableVector2 { X = vector.X, Y = vector.Y };
        }

        public override string ToString()
        {
            return $"<{X}, {Y}>";
        }
    }
}