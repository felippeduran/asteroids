using System.Linq;

namespace Gameplay.Simulation.Runtime
{
    public struct GameSystems
    {
        public ShipSystem ShipSystem;
        public BulletsSystem BulletsSystem;
        public SaucersSystem SaucersSystem;
        public AsteroidsSystem AsteroidsSystem;
        public WorldLoopSystem WorldLoopSystem;
        public ExtraLifeSystem ExtraLifeSystem;

        public void Update(float deltaTime, ref GameState gameState, GameConfig gameConfig)
        {
            var bulletsFired = ShipSystem.UpdateShip(deltaTime, gameState.PlayerShip, ref gameState.PlayerState, gameConfig.Ship, gameConfig.WorldBounds);
            var moreBulletsFired = SaucersSystem.UpdateSaucers(deltaTime, ref gameState.SaucersState, gameState.Saucers, gameConfig.Saucers, gameConfig.WorldBounds);
            bulletsFired = bulletsFired.Concat(moreBulletsFired).ToArray();
            BulletsSystem.UpdateBullets(deltaTime, bulletsFired, ref gameState.PlayerState, gameState.Bullets, gameConfig.Bullets);

            ExtraLifeSystem.UpdateExtraLives(ref gameState.PlayerState, gameConfig.Lives);

            AsteroidsSystem.UpdateAsteroids(deltaTime, ref gameState.WaveState, gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameConfig.Asteroids, gameConfig.WorldBounds);
            WorldLoopSystem.LoopObjectsThroughWorld(gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameState.Bullets, gameConfig.WorldBounds);
        }
    }
}