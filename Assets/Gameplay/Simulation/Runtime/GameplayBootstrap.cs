using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gameplay.Simulation.Runtime;

public class GameplayBootstrap : MonoBehaviour
{
    public event Action<GameState> OnUpdate = delegate { };

    [SerializeField] GameConfig gameConfig;
    [SerializeField] CameraGroup cameras;
    [SerializeField] GameState gameState;

    ObjectPool<Asteroid> asteroidsPool;
    ObjectPool<Bullet> bulletsPool;
    ObjectPool<Saucer> saucersPool;
    ShipController shipController;
    BulletsController bulletsController;
    SaucersController saucersController;

    public void Setup(GameState gameState, GameConfig gameConfig, CameraGroup cameras, ObjectPool<Asteroid> asteroidsPool, ObjectPool<Bullet> bulletsPool, ObjectPool<Saucer> saucersPool, ShipController shipController, BulletsController bulletsController, SaucersController saucersController)
    {
        this.gameState = gameState;
        this.gameConfig = gameConfig;
        this.cameras = cameras;
        this.asteroidsPool = asteroidsPool;
        this.bulletsPool = bulletsPool;
        this.saucersPool = saucersPool;
        this.shipController = shipController;
        this.bulletsController = bulletsController;
        this.saucersController = saucersController;
    }

    void Update()
    {
        var worldBounds = new Bounds(Vector2.zero, cameras.GetWorldSize());

        var bulletsFired = shipController.UpdateShip(gameState.PlayerShip, ref gameState.Player, gameConfig, worldBounds);

        UpdateAsteroids(Time.deltaTime, ref gameState, gameConfig, worldBounds);
        bulletsController.UpdateBullets(Time.deltaTime, bulletsFired, ref gameState, gameState.Bullets, gameConfig);
        saucersController.UpdateSaucers(ref gameState, gameConfig.Saucers, worldBounds);

        LoopObjectsThroughWorld(worldBounds, gameState);


        OnUpdate(gameState);
    }

    void OnDrawGizmos()
    {
        if (gameState.PlayerShip != null)
        {
            Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.SpawnRange.Min);
            Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.SpawnRange.Max);
        }
    }









    void HandleDestroyedAsteroids(GameState gameState)
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



    void LoopObjectsThroughWorld(Bounds worldBounds, GameState gameState)
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

    void UpdateAsteroids(float deltaTime, ref GameState gameState, GameConfig gameConfig, Bounds worldBounds)
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

        if (gameState.Saucers.Count == 0 && gameState.Asteroids.Count == 0 && !gameState.NextWave)
        {
            Debug.Log("Schedule next wave");
            gameState.NextWave = true;
            gameState.NextWaveCooldown = gameConfig.NextWaveCooldown;
        }

        HandleDestroyedAsteroids(gameState);
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