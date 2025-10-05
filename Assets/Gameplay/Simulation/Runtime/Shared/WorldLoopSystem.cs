
using System.Numerics;
using System.Collections.Generic;

namespace Gameplay.Simulation.Runtime
{
    public class WorldLoopSystem
    {
        public WorldLoopSystem() { }

        public void LoopObjectsThroughWorld(IShip playerShip, ISet<IAsteroid> existingAsteroids, List<ISaucer> existingSaucers, List<IBullet> existingBullets, Bounds worldBounds)
        {
            foreach (var asteroid in existingAsteroids)
            {
                asteroid.Position = GetLoopedPosition(asteroid.Position, worldBounds);
            }

            foreach (var saucer in existingBullets)
            {
                saucer.Position = GetLoopedPosition(saucer.Position, worldBounds);
            }

            foreach (var bullet in existingBullets)
            {
                bullet.Position = GetLoopedPosition(bullet.Position, worldBounds);
            }

            playerShip.Position = GetLoopedPosition(playerShip.Position, worldBounds);
        }

        Vector2 GetLoopedPosition(Vector2 position, Bounds bounds)
        {
            float xPosition = position.X;
            if (position.X > bounds.Max.X)
            {
                xPosition = bounds.Min.X + (position.X - bounds.Max.X);
            }
            else if (position.X < bounds.Min.X)
            {
                xPosition = bounds.Max.X - (bounds.Min.X - position.X);
            }

            float yPosition = position.Y;
            if (position.Y > bounds.Max.Y)
            {
                yPosition = bounds.Min.Y + (position.Y - bounds.Max.Y);
            }
            else if (position.Y < bounds.Min.Y)
            {
                yPosition = bounds.Max.Y - (bounds.Min.Y - position.Y);
            }

            return new Vector2(xPosition, yPosition);
        }
    }
}
