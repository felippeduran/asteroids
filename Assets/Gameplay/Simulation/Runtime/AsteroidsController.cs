using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    public class AsteroidsController
    {
        readonly ObjectPool<Asteroid> asteroidsPool;

        public AsteroidsController(ObjectPool<Asteroid> asteroidsPool)
        {
            this.asteroidsPool = asteroidsPool;
        }

        public void UpdateAsteroids(float deltaTime, ref GameState gameState, GameConfig gameConfig, Bounds worldBounds)
        {
            HandleWaveSpawning(deltaTime, ref gameState, gameConfig, worldBounds);
            HandleWaveScheduling(ref gameState, gameConfig);
            HandleDestroyedAsteroids(ref gameState, gameConfig);
        }

        void HandleWaveSpawning(float deltaTime, ref GameState gameState, GameConfig gameConfig, Bounds worldBounds)
        {
            if (gameState.NextWave)
            {
                gameState.NextWaveCooldown -= deltaTime;
                if (gameState.NextWaveCooldown < 0f)
                {
                    Debug.Log("Next wave!");
                    gameState.NextWave = false;
                    gameState.Wave++;
                    var numberOfAsteroids = gameConfig.InitialAsteroids + (gameState.Wave - 1) * gameConfig.ExtraAsteroidsPerWave;
                    for (int i = 0; i < numberOfAsteroids; i++)
                    {
                        Asteroid asteroid = asteroidsPool.Get();

                        asteroid.Type = AsteroidType.Large;
                        asteroid.ScoreWorth = gameConfig.Scoring.Asteroids.GetScoreFor(AsteroidType.Large);
                        asteroid.Position = GetRandomAsteroidSpawnPosition(gameState.PlayerShip.Position, gameConfig.SpawnRange, worldBounds);
                        asteroid.LinearVelocity = GetRandomAsteroidSpawnVelocity(gameConfig.AsteroidSpeedRange);
                        gameState.Asteroids.Add(asteroid);
                    }
                }
            }
        }

        void HandleWaveScheduling(ref GameState gameState, GameConfig gameConfig)
        {
            if (gameState.Saucers.Count == 0 && gameState.Asteroids.Count == 0 && !gameState.NextWave)
            {
                Debug.Log("Schedule next wave");
                gameState.NextWave = true;
                gameState.NextWaveCooldown = gameConfig.NextWaveCooldown;
            }
        }

        void HandleDestroyedAsteroids(ref GameState gameState, GameConfig gameConfig)
        {
            List<Asteroid> asteroidsToRemove = new();
            List<Asteroid> asteroidsToAdd = new();
            foreach (var asteroid in gameState.Asteroids)
            {
                if (asteroid.IsDestroyed)
                {
                    asteroidsToRemove.Add(asteroid);
                    // TODO: Trigger particle effect

                    if (asteroid.Type == AsteroidType.Large)
                    {
                        var linearVelocity = asteroid.LinearVelocity;

                        var random = new Random();

                        for (int i = 0; i < 2; i++)
                        {
                            var randomVelocity = GetRandomVelocityFromCone(linearVelocity, gameConfig.AsteroidSplitAngle, gameConfig.AsteroidSpeedRange);

                            var asteroidConfig = gameConfig.Scoring.Asteroids.GetScoreConfigFor(AsteroidType.Medium);
                            var newAsteroid = SpawnAsteroid(asteroid.Position, randomVelocity, asteroidConfig);
                            asteroidsToAdd.Add(newAsteroid);
                        }
                    }
                    else if (asteroid.Type == AsteroidType.Medium)
                    {
                        var linearVelocity = asteroid.LinearVelocity;

                        // Get random direction within a cone around the asteroid's forward direction
                        var random = new Random();

                        for (int i = 0; i < 2; i++)
                        {
                            var randomVelocity = GetRandomVelocityFromCone(linearVelocity, gameConfig.AsteroidSplitAngle, gameConfig.AsteroidSpeedRange);

                            var asteroidConfig = gameConfig.Scoring.Asteroids.GetScoreConfigFor(AsteroidType.Small);
                            var newAsteroid = SpawnAsteroid(asteroid.Position, randomVelocity, asteroidConfig);
                            asteroidsToAdd.Add(newAsteroid);
                        }
                    }
                }
            }

            foreach (var asteroid in asteroidsToRemove)
            {
                asteroidsPool.Add(asteroid);
                gameState.Asteroids.Remove(asteroid);
            }

            foreach (var asteroid in asteroidsToAdd)
            {
                gameState.Asteroids.Add(asteroid);
            }
        }

        Vector2 GetRandomVelocityFromCone(Vector2 linearVelocity, float angle, FloatRange speedRange)
        {
            var random = new Random();
            var randomDirection = GetRandomDirectionFromCone(linearVelocity, angle);
            var randomSpeed = random.Range(speedRange.Min, speedRange.Max);
            return randomSpeed * randomDirection;
        }

        Vector2 GetRandomDirectionFromCone(Vector2 linearVelocity, float angle)
        {
            var random = new Random();
            var randomAngle = random.Range(-angle / 2, angle / 2);
            var randomDirection = Quaternion.Euler(0, 0, randomAngle) * linearVelocity.normalized;
            return randomDirection;
        }

        Asteroid SpawnAsteroid(Vector2 position, Vector2 velocity, AsteroidScoreConfig asteroidConfig)
        {
            var newAsteroid = asteroidsPool.Get();
            newAsteroid.Type = asteroidConfig.Type;
            newAsteroid.ScoreWorth = asteroidConfig.Score;
            newAsteroid.Position = position;
            newAsteroid.LinearVelocity = velocity;
            newAsteroid.IsDestroyed = false;

            return newAsteroid;
        }

        Vector2 GetRandomAsteroidSpawnVelocity(FloatRange speedRange)
        {
            var random = new Random();
            var speed = speedRange.Min + speedRange.Length * random.NextFloat();
            return speed * random.NextDirection().normalized;
        }

        Vector2 GetRandomAsteroidSpawnPosition(Vector2 playerPosition, FloatRange spawnRange, Bounds worldBounds)
        {
            var random = new Random();

            var direction = random.NextDirection();
            var extra = random.NextFloat() * spawnRange.Length;

            return GetLoopedPosition(playerPosition + direction * (float)(extra + spawnRange.Min), worldBounds);
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
