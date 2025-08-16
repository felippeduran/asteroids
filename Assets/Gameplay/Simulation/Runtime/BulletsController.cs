using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    public class BulletsController
    {
        readonly ObjectPool<Bullet> bulletsPool;
        readonly GameState gameState;

        public BulletsController(ObjectPool<Bullet> bulletsPool, GameState gameState)
        {
            this.bulletsPool = bulletsPool;
            this.gameState = gameState;
        }

        public void UpdateBullets(float deltaTime, FireBulletData[] bulletsFired, List<Bullet> existingBullets, GameConfig gameConfig)
        {
            foreach (var bulletData in bulletsFired)
            {
                var bullet = SpawnBullet(bulletData, gameConfig);
                existingBullets.Add(bullet);
            }

            foreach (var bullet in existingBullets)
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

        Bullet SpawnBullet(FireBulletData bulletData, GameConfig gameConfig)
        {
            Bullet bullet = bulletsPool.Get();

            bullet.IsDestroyed = false;
            bullet.Position = bulletData.Position;
            bullet.LinearVelocity = gameConfig.BulletSpeed * bulletData.Forward;
            bullet.TotalTraveledDistance = 0f;
            bullet.Score = 0;
            bullet.IsPlayerBullet = bulletData.IsPlayerBullet;
            Debug.Log($"Spawned bullet at {bullet.Position} with velocity {bullet.LinearVelocity}, forward {bulletData.Forward}");

            return bullet;
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
    }
}
