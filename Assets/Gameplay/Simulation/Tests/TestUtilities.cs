using UnityEngine;

namespace Gameplay.Simulation.Tests
{
    public static class TestUtilities
    {
        public static Bounds CreateWorldBounds()
        {
            return new Bounds(Vector3.zero, new Vector3(20f, 20f, 0f));
        }
    }
}