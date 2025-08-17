using System;
using UnityEngine;
using Gameplay.Simulation.Runtime;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

public struct GameSystems
{
    public ShipController ShipController;
    public BulletsController BulletsController;
    public SaucersController SaucersController;
    public AsteroidsController AsteroidsController;
    public WorldLoopController WorldLoopController;
}

public class GameplayBootstrap : MonoBehaviour, IDisposable
{
    public event Action<GameState> OnUpdate = delegate { };

    [SerializeField] GameConfig gameConfig;
    [SerializeField] GameState gameState;

    GameSystems gameSystems;
    GameplayDisposer gameplayDisposer;

    public void Setup(GameState gameState, GameConfig gameConfig, GameSystems gameSystems, GameplayDisposer gameplayDisposer)
    {
        this.gameState = gameState;
        this.gameConfig = gameConfig;
        this.gameSystems = gameSystems;
        this.gameplayDisposer = gameplayDisposer;
    }

    public async Task WaitForCompletionAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && !gameState.PlayerState.GameOver)
        {
            await Task.Yield();
        }
    }

    public void Dispose()
    {
        gameplayDisposer.Dispose();
    }

    void Update()
    {
        var bulletsFired = gameSystems.ShipController.UpdateShip(Time.deltaTime, gameState.PlayerShip, ref gameState.PlayerState, gameConfig.Ship, gameConfig.WorldBounds);
        var moreBulletsFired = gameSystems.SaucersController.UpdateSaucers(Time.deltaTime, ref gameState.SaucersState, gameState.Saucers, gameConfig.Saucers, gameConfig.WorldBounds);
        bulletsFired = bulletsFired.Concat(moreBulletsFired).ToArray();
        gameSystems.BulletsController.UpdateBullets(Time.deltaTime, bulletsFired, ref gameState.PlayerState, gameState.Bullets, gameConfig.Bullets);

        gameSystems.AsteroidsController.UpdateAsteroids(Time.deltaTime, ref gameState.WaveState, gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameConfig.Asteroids, gameConfig.WorldBounds);
        gameSystems.WorldLoopController.LoopObjectsThroughWorld(gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameState.Bullets, gameConfig.WorldBounds);


        OnUpdate(gameState);
    }

    void OnDrawGizmos()
    {
        if (gameState.PlayerShip != null)
        {
            Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.Asteroids.SpawnRange.Min);
            Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.Asteroids.SpawnRange.Max);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, gameConfig.WorldSize);
    }














}