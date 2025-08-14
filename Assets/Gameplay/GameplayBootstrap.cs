using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayBootstrap : MonoBehaviour
{
    [Serializable]
    public struct FloatRange
    {
        public float Min;
        public float Max;
        public float Length => Max - Min;
    }

    [Serializable]
    public struct GameConfig
    {
        public int InitialLives;
        public ShipConfig Ship;
        public float NextWaveCooldown;
        public int InitialAsteroids;
        public int ExtraAsteroidsPerWave;
        public FloatRange SpawnRange;
        public FloatRange AsteroidSpeedRange;
    }

    [Serializable]
    public struct GameState
    {
        public int Lives;
        public int Score;
        public int Wave;
        public bool NextWave;
        public float NextWaveCooldown;
        public Ship PlayerShip;
        public List<Asteroid> Asteroids;
        public List<Saucer> Saucers;
        public List<Bullet> Bullets;
    }

    [SerializeField] GameConfig gameConfig;
    [SerializeField] Ship shipPrefab;
    [SerializeField] Asteroid asteroidPrefab;

    [SerializeField] CameraGroup cameras;
    [SerializeField] GameState gameState;
    [SerializeField] List<Asteroid> asteroidsPool;

    void Start()
    {
        var playerShip = Instantiate(shipPrefab);
        playerShip.Setup(gameConfig.Ship);
        playerShip.Position = new Vector2(0, 0);

        gameState = new GameState
        {
            Lives = gameConfig.InitialLives,
            Score = 0,
            Wave = 0,
            PlayerShip = playerShip,
            Asteroids = new List<Asteroid>(),
            Saucers = new List<Saucer>(),
            Bullets = new List<Bullet>(),
        };
    }

    void Update()
    {
        var worldBounds = new Bounds(Vector2.zero, cameras.GetWorldSize());

        ResetShip(gameState.PlayerShip);
        UpdateShip(gameState.PlayerShip);
        UpdateWave(Time.deltaTime, ref gameState, gameConfig, worldBounds);

        LoopObjectsThroughWorld(worldBounds, gameState);
    }

    void OnDrawGizmos()
    {
        if (gameState.PlayerShip != null)
        {
            Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.SpawnRange.Min);
            Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.SpawnRange.Max);
        }
    }

    void ResetShip(IShip ship)
    {
        ship.Reset();
    }

    void UpdateShip(IShip ship)
    {
        if (Input.GetKey(KeyCode.A))
        {
            ship.TurnLeft();
        }

        if (Input.GetKey(KeyCode.D))
        {
            ship.TurnRight();
        }

        if (Input.GetKey(KeyCode.W))
        {
            ship.Thrust();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // TODO: Fire a bullet
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            // TODO: Teleport the ship to a random position
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

    void UpdateWave(float deltaTime, ref GameState gameState, GameConfig gameConfig, Bounds worldBounds)
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
                    Asteroid asteroid;
                    if (asteroidsPool.Count > 0)
                    {
                        var lastIndex = asteroidsPool.Count - 1;
                        asteroid = asteroidsPool[lastIndex];
                        asteroidsPool.RemoveAt(lastIndex);
                    }
                    else
                    {
                        asteroid = Instantiate(asteroidPrefab);
                    }

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