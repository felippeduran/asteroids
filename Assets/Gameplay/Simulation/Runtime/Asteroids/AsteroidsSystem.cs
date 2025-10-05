using System.Numerics;
using System.Collections.Generic;
using Company.Utilities.Runtime;
using Random = Company.Utilities.Runtime.Random;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime
{
    public class AsteroidsSystem
    {
        readonly IObjectPool<IAsteroid> asteroidsPool;

        public AsteroidsSystem(IObjectPool<IAsteroid> asteroidsPool)
        {
            this.asteroidsPool = asteroidsPool;
        }

        public void UpdateAsteroids(float deltaTime, ref AsteroidWaveState waveState, IShip playerShip, ISet<IAsteroid> existingAsteroids, List<ISaucer> existingSaucers, AsteroidsConfig asteroidsConfig, Bounds worldBounds)
        {
            HandleWaveSpawning(deltaTime, ref waveState, playerShip, existingAsteroids, asteroidsConfig, worldBounds);
            HandleWaveScheduling(ref waveState, existingSaucers, existingAsteroids, asteroidsConfig);
            HandleDestroyedAsteroids(existingAsteroids, asteroidsConfig);
        }

        void HandleWaveSpawning(float deltaTime, ref AsteroidWaveState waveState, IShip playerShip, ISet<IAsteroid> existingAsteroids, AsteroidsConfig asteroidsConfig, Bounds worldBounds)
        {
            if (waveState.NextWave)
            {
                waveState.NextWaveCooldown -= deltaTime;
                if (waveState.NextWaveCooldown < 0f)
                {
                    Logger.Log("Next wave!");
                    waveState.NextWave = false;
                    waveState.Current++;
                    var numberOfAsteroids = asteroidsConfig.Initial + (waveState.Current - 1) * asteroidsConfig.ExtraPerWave;
                    var playerShipPosition = GetPlayerShipPosition(playerShip);

                    var asteroids = RandomlySpawnAsteroids(numberOfAsteroids, playerShipPosition, asteroidsConfig, worldBounds);
                    existingAsteroids.UnionWith(asteroids);
                }
            }
        }

        Vector2 GetPlayerShipPosition(IShip playerShip)
        {
            var position = playerShip.Position;
            if (playerShip.IsDestroyed)
            {
                position = new Vector2(0, 0);
            }

            return position;
        }

        IAsteroid[] RandomlySpawnAsteroids(int numberOfAsteroids, Vector2 playerShipPosition, AsteroidsConfig asteroidsConfig, Bounds worldBounds)
        {
            var asteroids = new IAsteroid[numberOfAsteroids];
            for (int i = 0; i < numberOfAsteroids; i++)
            {
                var spawnPosition = GetRandomAsteroidSpawnPosition(playerShipPosition, asteroidsConfig.SpawnRange, worldBounds);
                var spawnVelocity = GetRandomAsteroidSpawnVelocity(asteroidsConfig.SpeedRange);

                var scoreConfig = asteroidsConfig.Scores.GetScoreConfigFor(AsteroidType.Large);
                var asteroid = SpawnAsteroid(spawnPosition, spawnVelocity, scoreConfig);
                asteroids[i] = asteroid;
            }

            return asteroids;
        }

        void HandleWaveScheduling(ref AsteroidWaveState waveState, List<ISaucer> existingSaucers, ISet<IAsteroid> existingAsteroids, AsteroidsConfig asteroidsConfig)
        {
            if (existingSaucers.Count == 0 && existingAsteroids.Count == 0 && !waveState.NextWave)
            {
                Logger.Log("Schedule next wave");
                waveState.NextWave = true;
                waveState.NextWaveCooldown = asteroidsConfig.NextWaveCooldown;
            }
        }

        void HandleDestroyedAsteroids(ISet<IAsteroid> existingAsteroids, AsteroidsConfig asteroidsConfig)
        {
            List<IAsteroid> asteroidsToRemove = new();
            List<IAsteroid> asteroidsToAdd = new();
            foreach (var asteroid in existingAsteroids)
            {
                if (asteroid.IsDestroyed)
                {
                    asteroidsToRemove.Add(asteroid);

                    if (asteroid.Type == AsteroidType.Large)
                    {
                        var linearVelocity = asteroid.LinearVelocity;

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

        IAsteroid SpawnAsteroid(Vector2 position, Vector2 velocity, AsteroidScoreConfig asteroidConfig)
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
            return speed * Vector2.Normalize(random.NextDirection());
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
