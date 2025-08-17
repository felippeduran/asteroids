using System.Collections.Generic;
using UnityEngine;
using Random = Company.Utilities.Runtime.Random;

namespace Gameplay.Simulation.Runtime
{
    public class AsteroidsController
    {
        readonly ObjectPool<Asteroid> asteroidsPool;

        public AsteroidsController(ObjectPool<Asteroid> asteroidsPool)
        {
            this.asteroidsPool = asteroidsPool;
        }

        public void UpdateAsteroids(float deltaTime, ref AsteroidWaveState waveState, IShip playerShip, ISet<Asteroid> existingAsteroids, List<Saucer> existingSaucers, AsteroidsConfig asteroidsConfig, Bounds worldBounds)
        {
            HandleWaveSpawning(deltaTime, ref waveState, playerShip, existingAsteroids, asteroidsConfig, worldBounds);
            HandleWaveScheduling(ref waveState, existingSaucers, existingAsteroids, asteroidsConfig);
            HandleDestroyedAsteroids(existingAsteroids, asteroidsConfig);
        }

        void HandleWaveSpawning(float deltaTime, ref AsteroidWaveState waveState, IShip playerShip, ISet<Asteroid> existingAsteroids, AsteroidsConfig asteroidsConfig, Bounds worldBounds)
        {
            if (waveState.NextWave)
            {
                waveState.NextWaveCooldown -= deltaTime;
                if (waveState.NextWaveCooldown < 0f)
                {
                    Debug.Log("Next wave!");
                    waveState.NextWave = false;
                    waveState.Current++;
                    var numberOfAsteroids = asteroidsConfig.Initial + (waveState.Current - 1) * asteroidsConfig.ExtraPerWave;
                    for (int i = 0; i < numberOfAsteroids; i++)
                    {
                        var spawnPosition = GetRandomAsteroidSpawnPosition(playerShip.Position, asteroidsConfig.SpawnRange, worldBounds);
                        var spawnVelocity = GetRandomAsteroidSpawnVelocity(asteroidsConfig.SpeedRange);

                        var scoreConfig = asteroidsConfig.Scores.GetScoreConfigFor(AsteroidType.Large);
                        var asteroid = SpawnAsteroid(spawnPosition, spawnVelocity, scoreConfig);
                        existingAsteroids.Add(asteroid);
                    }
                }
            }
        }

        void HandleWaveScheduling(ref AsteroidWaveState waveState, List<Saucer> existingSaucers, ISet<Asteroid> existingAsteroids, AsteroidsConfig asteroidsConfig)
        {
            if (existingSaucers.Count == 0 && existingAsteroids.Count == 0 && !waveState.NextWave)
            {
                Debug.Log("Schedule next wave");
                waveState.NextWave = true;
                waveState.NextWaveCooldown = asteroidsConfig.NextWaveCooldown;
            }
        }

        void HandleDestroyedAsteroids(ISet<Asteroid> existingAsteroids, AsteroidsConfig asteroidsConfig)
        {
            List<Asteroid> asteroidsToRemove = new();
            List<Asteroid> asteroidsToAdd = new();
            foreach (var asteroid in existingAsteroids)
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
                            var randomVelocity = GetRandomVelocityFromCone(linearVelocity, asteroidsConfig.SplitAngle, asteroidsConfig.SpeedRange);

                            var asteroidConfig = asteroidsConfig.Scores.GetScoreConfigFor(AsteroidType.Medium);
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
                            var randomVelocity = GetRandomVelocityFromCone(linearVelocity, asteroidsConfig.SplitAngle, asteroidsConfig.SpeedRange);

                            var asteroidConfig = asteroidsConfig.Scores.GetScoreConfigFor(AsteroidType.Small);
                            var newAsteroid = SpawnAsteroid(asteroid.Position, randomVelocity, asteroidConfig);
                            asteroidsToAdd.Add(newAsteroid);
                        }
                    }
                }
            }

            foreach (var asteroid in asteroidsToRemove)
            {
                asteroidsPool.Add(asteroid);
                existingAsteroids.Remove(asteroid);
            }

            foreach (var asteroid in asteroidsToAdd)
            {
                existingAsteroids.Add(asteroid);
            }
        }

        Vector2 GetRandomVelocityFromCone(Vector2 linearVelocity, float angle, FloatRange speedRange)
        {
            var random = new Random();
            var randomDirection = random.GetRandomDirectionFromCone(linearVelocity, angle);
            var randomSpeed = random.Range(speedRange.Min, speedRange.Max);
            return randomSpeed * randomDirection;
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
