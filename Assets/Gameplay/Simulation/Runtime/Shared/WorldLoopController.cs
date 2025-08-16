using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    public class WorldLoopController
    {
        public WorldLoopController() { }

        public void LoopObjectsThroughWorld(ref GameState gameState, Bounds worldBounds)
        {
            foreach (var asteroid in gameState.Asteroids)
            {
                asteroid.Position = GetLoopedPosition(asteroid.Position, worldBounds);
            }

            foreach (var saucer in gameState.Saucers)
            {
                saucer.Position = GetLoopedPosition(saucer.Position, worldBounds);
            }

            foreach (var bullet in gameState.Bullets)
            {
                bullet.Position = GetLoopedPosition(bullet.Position, worldBounds);
            }

            gameState.PlayerShip.Position = GetLoopedPosition(gameState.PlayerShip.Position, worldBounds);
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
