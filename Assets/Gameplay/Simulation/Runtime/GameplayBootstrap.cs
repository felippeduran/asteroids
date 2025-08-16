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

    public void Setup(GameState gameState, GameConfig gameConfig, CameraGroup cameras, ObjectPool<Asteroid> asteroidsPool, ObjectPool<Bullet> bulletsPool, ObjectPool<Saucer> saucersPool, ShipController shipController)
    {
        this.gameState = gameState;
        this.gameConfig = gameConfig;
        this.cameras = cameras;
        this.asteroidsPool = asteroidsPool;
        this.bulletsPool = bulletsPool;
        this.saucersPool = saucersPool;
        this.shipController = shipController;
    }

    void Update()
    {
        var worldBounds = new Bounds(Vector2.zero, cameras.GetWorldSize());

        var bulletsFired = shipController.UpdateShip(gameState.PlayerShip, ref gameState.Player, gameConfig, worldBounds);

        UpdateAsteroids(Time.deltaTime, ref gameState, gameConfig, worldBounds);
        UpdateBullets(Time.deltaTime, bulletsFired, gameState.Bullets, gameConfig);
        UpdateSaucers(gameConfig.Saucers, ref gameState, worldBounds);

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



    void UpdateSaucers(SaucersConfig saucersConfig, ref GameState gameState, Bounds worldBounds)
    {
        var random = new Random();

        gameState.SaucerSpawnCooldown -= Time.deltaTime;
        if (gameState.SaucerSpawnCooldown < 0f)
        {
            Debug.Log("Try spawning saucers");
            gameState.SaucerSpawnCooldown = saucersConfig.SpawnCooldown;
            foreach (var spawnRate in saucersConfig.SpawnConfigs)
            {
                if (!gameState.Saucers.Any(x => x.Type == spawnRate.Type) && random.NextFloat() < spawnRate.Chance)
                {
                    var saucer = SpawnSaucer(saucersConfig.Saucers.GetSaucerConfigFor(spawnRate.Type), worldBounds);
                    gameState.Saucers.Add(saucer);
                }
            }
        }

        foreach (var saucer in gameState.Saucers)
        {
            saucer.TurnCooldown -= Time.deltaTime;
            if (saucer.TurnCooldown < 0f)
            {
                Debug.Log($"Try turn {saucer.Type} saucer");
                var saucerConfig = saucersConfig.Saucers.GetSaucerConfigFor(saucer.Type);
                saucer.TurnCooldown = saucerConfig.Movement.TurnCooldown;
                if (random.NextFloat() < saucerConfig.Movement.TurnChance)
                {
                    var newVerticalComponent = random.NextFloat() switch
                    {
                        < 0.33f => 1f,
                        < 0.66f => 0f,
                        _ => -1f
                    };

                    saucer.LinearVelocity = new Vector2(Mathf.Sign(saucer.LinearVelocity.x), newVerticalComponent).normalized * saucerConfig.Movement.Speed;
                }
            }
        }

        var saucersToRemove = new List<Saucer>();
        foreach (var saucer in gameState.Saucers)
        {
            if (saucer.LinearVelocity.x > 0 && saucer.Position.x > worldBounds.max.x)
            {
                Debug.Log($"Saucer {saucer.Type} crossed the screen on the right");
                saucersToRemove.Add(saucer);
            }
            else if (saucer.LinearVelocity.x < 0 && saucer.Position.x < worldBounds.min.x)
            {
                Debug.Log($"Saucer {saucer.Type} crossed the screen on the left");
                saucersToRemove.Add(saucer);
            }
        }

        foreach (var saucer in saucersToRemove)
        {
            saucersPool.Add(saucer);
            gameState.Saucers.Remove(saucer);
        }

        HandleDestroyedSaucers(gameState);
    }

    void HandleDestroyedSaucers(GameState gameState)
    {
        var saucersToRemove = new List<Saucer>();
        foreach (var saucer in gameState.Saucers)
        {
            if (saucer.IsDestroyed)
            {
                saucersToRemove.Add(saucer);
            }
        }

        foreach (var saucer in saucersToRemove)
        {
            saucersPool.Add(saucer);
            gameState.Saucers.Remove(saucer);
        }
    }

    Saucer SpawnSaucer(SaucerConfig saucerConfig, Bounds worldBounds)
    {
        var random = new Random();

        var saucer = saucersPool.Get();

        saucer.Type = saucerConfig.Type;

        var direction = random.NextFloat() > 0.5f ? 1 : -1;
        saucer.Position = new Vector2(direction < 0f ? worldBounds.size.x : 0, random.NextFloat() * worldBounds.size.y) - new Vector2(worldBounds.size.x, worldBounds.size.y) / 2;
        saucer.LinearVelocity = new Vector2(direction, 0) * saucerConfig.Movement.Speed;
        saucer.TurnCooldown = random.NextFloat() * saucerConfig.Movement.TurnCooldown;
        saucer.IsDestroyed = false;

        return saucer;
    }

    void UpdateBullets(float deltaTime, FireBulletData[] bulletsFired, List<Bullet> bullets, GameConfig gameConfig)
    {
        foreach (var bulletFired in bulletsFired)
        {
            SpawnBullet(bulletFired.Position, bulletFired.Forward, gameConfig);
        }

        foreach (var bullet in bullets)
        {
            if (!bullet.IsDestroyed)
            {
                bullet.TotalTraveledDistance += bullet.LinearVelocity.magnitude * deltaTime;
                if (bullet.TotalTraveledDistance > gameConfig.BulletTravelDistance)
                {
                    bullet.IsDestroyed = true;
                }
            }
        }

        HandleDestroyedBullets(gameState);
    }

    Bullet SpawnBullet(Vector2 position, Vector2 forward, GameConfig gameConfig)
    {
        Bullet bullet = bulletsPool.Get();

        bullet.IsDestroyed = false;
        bullet.Position = position;
        bullet.LinearVelocity = gameConfig.BulletSpeed * forward;
        bullet.TotalTraveledDistance = 0f;
        bullet.Score = 0;
        gameState.Bullets.Add(bullet);
        Debug.Log($"Spawned bullet at {bullet.Position} with velocity {bullet.LinearVelocity}, forward {forward}");

        return bullet;
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

    void HandleDestroyedBullets(GameState gameState)
    {
        List<Bullet> bulletsToRemove = new();
        foreach (var bullet in gameState.Bullets)
        {
            if (bullet.IsDestroyed)
            {
                gameState.Player.Score += bullet.Score;
                bulletsToRemove.Add(bullet);
            }
        }

        foreach (var bullet in bulletsToRemove)
        {
            bulletsPool.Add(bullet);
            gameState.Bullets.Remove(bullet);
        }
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