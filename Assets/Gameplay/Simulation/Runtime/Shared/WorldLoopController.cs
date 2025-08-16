using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Simulation.Runtime
{
    public class WorldLoopController
    {
        public WorldLoopController() { }

        public void LoopObjectsThroughWorld(IShip playerShip, ISet<Asteroid> existingAsteroids, List<Saucer> existingSaucers, List<Bullet> existingBullets, Bounds worldBounds)
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
            float xPosition = position.x;
            if (position.x > bounds.max.x)
            {
                xPosition = bounds.min.x + (position.x - bounds.max.x);
            }
            else if (position.x < bounds.min.x)
            {
                xPosition = bounds.max.x - (bounds.min.x - position.x);
            }

            float yPosition = position.y;
            if (position.y > bounds.max.y)
            {
                yPosition = bounds.min.y + (position.y - bounds.max.y);
            }
            else if (position.y < bounds.min.y)
            {
                yPosition = bounds.max.y - (bounds.min.y - position.y);
            }

            return new Vector2(xPosition, yPosition);
        }
    }
}
