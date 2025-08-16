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
    [SerializeField] CameraGroup cameras;
    [SerializeField] GameState gameState;

    GameSystems gameSystems;

    public void Setup(GameState gameState, GameConfig gameConfig, CameraGroup cameras, GameSystems gameSystems)
    {
        this.gameState = gameState;
        this.gameConfig = gameConfig;
        this.cameras = cameras;
        this.gameSystems = gameSystems;
    }

    void Update()
    {
        var worldBounds = new Bounds(Vector2.zero, cameras.GetWorldSize());

        var bulletsFired = gameSystems.ShipController.UpdateShip(gameState.PlayerShip, ref gameState.Player, gameConfig, worldBounds);
        var moreBulletsFired = gameSystems.SaucersController.UpdateSaucers(Time.deltaTime, ref gameState, gameConfig.Saucers, worldBounds);
        bulletsFired = bulletsFired.Concat(moreBulletsFired).ToArray();
        gameSystems.BulletsController.UpdateBullets(Time.deltaTime, bulletsFired, ref gameState, gameState.Bullets, gameConfig);

        gameSystems.AsteroidsController.UpdateAsteroids(Time.deltaTime, ref gameState, gameConfig, worldBounds);
        gameSystems.WorldLoopController.LoopObjectsThroughWorld(ref gameState, worldBounds);


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














}