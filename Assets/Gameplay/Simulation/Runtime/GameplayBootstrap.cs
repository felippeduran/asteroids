using System;
using UnityEngine;
using Gameplay.Simulation.Runtime;
using System.Linq;

public struct GameSystems
{
    public ShipController ShipController;
    public BulletsController BulletsController;
    public SaucersController SaucersController;
    public AsteroidsController AsteroidsController;
    public WorldLoopController WorldLoopController;
}

public class GameplayBootstrap : MonoBehaviour
{
    public event Action<GameState> OnUpdate = delegate { };

    [SerializeField] GameConfig gameConfig;
    [SerializeField] GameState gameState;

    GameSystems gameSystems;

    public void Setup(GameState gameState, GameConfig gameConfig, GameSystems gameSystems)
    {
        this.gameState = gameState;
        this.gameConfig = gameConfig;
        this.gameSystems = gameSystems;
    }

    void Update()
    {
        var worldBounds = new Bounds(Vector2.zero, gameConfig.WorldSize);

        var bulletsFired = gameSystems.ShipController.UpdateShip(Time.deltaTime, gameState.PlayerShip, ref gameState.PlayerState, gameConfig.Ship, worldBounds);
        var moreBulletsFired = gameSystems.SaucersController.UpdateSaucers(Time.deltaTime, ref gameState.SaucersState, gameState.Saucers, gameConfig.Saucers, worldBounds);
        bulletsFired = bulletsFired.Concat(moreBulletsFired).ToArray();
        gameSystems.BulletsController.UpdateBullets(Time.deltaTime, bulletsFired, ref gameState.PlayerState, gameState.Bullets, gameConfig.Bullets);

        gameSystems.AsteroidsController.UpdateAsteroids(Time.deltaTime, ref gameState.WaveState, gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameConfig.Asteroids, worldBounds);
        gameSystems.WorldLoopController.LoopObjectsThroughWorld(gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameState.Bullets, worldBounds);


        OnUpdate(gameState);
    }

    void OnDrawGizmos()
    {
        if (gameState.PlayerShip != null)
        {
            Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.Asteroids.SpawnRange.Min);
            Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.Asteroids.SpawnRange.Max);
        }
    }














}