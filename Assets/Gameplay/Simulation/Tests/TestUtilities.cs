using System.Numerics;
using Gameplay.Simulation.Runtime;

namespace Gameplay.Simulation.Tests
{
    public static class TestUtilities
    {
        public static Bounds CreateWorldBounds()
        {
            return new Bounds(Vector2.Zero, new Vector2(20f, 20f));
        }
    }
}