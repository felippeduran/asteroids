using System.Collections.Generic;
using System.Numerics;
using Company.Utilities.Runtime;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime
{
    public struct FireBulletData
    {
        public Vector2 Position;
        public Vector2 Forward;
        public bool IsPlayerBullet;
    }

    public class BulletsSystem
    {
        readonly IObjectPool<IBullet> bulletsPool;

        public BulletsSystem(IObjectPool<IBullet> bulletsPool)
        {
            this.bulletsPool = bulletsPool;
        }

        public void UpdateBullets(float deltaTime, FireBulletData[] bulletsFired, ref PlayerState playerState, List<IBullet> existingBullets, BulletsConfig bulletsConfig)
        {
            var bullets = SpawnBullets(bulletsFired, bulletsConfig);
            existingBullets.AddRange(bullets);

            UpdateTraveledBullets(deltaTime, existingBullets, bulletsConfig);
            ExpireTraveledBullets(existingBullets, bulletsConfig);

            HandleDestroyedBullets(ref playerState, existingBullets);
        }

        IBullet[] SpawnBullets(FireBulletData[] bulletsFired, BulletsConfig bulletsConfig)
        {
            var bullets = new IBullet[bulletsFired.Length];
            for (int i = 0; i < bulletsFired.Length; i++)
            {
                bullets[i] = SpawnBullet(bulletsFired[i], bulletsConfig);
            }

            return bullets;
        }

        void UpdateTraveledBullets(float deltaTime, List<IBullet> existingBullets, BulletsConfig bulletsConfig)
        {
            foreach (var bullet in existingBullets)
            {
                bullet.TotalTraveledDistance += bullet.LinearVelocity.Magnitude() * deltaTime;
            }
        }

        void ExpireTraveledBullets(List<IBullet> existingBullets, BulletsConfig bulletsConfig)
        {
            foreach (var bullet in existingBullets)
            {
                if (bullet.TotalTraveledDistance > bulletsConfig.TravelDistance)
                {
                    bullet.IsDestroyed = true;
                }
            }
        }

        IBullet SpawnBullet(FireBulletData bulletData, BulletsConfig bulletsConfig)
        {
            IBullet bullet = bulletsPool.Get();

            bullet.IsDestroyed = false;
            bullet.Position = bulletData.Position;
            bullet.LinearVelocity = bulletsConfig.Speed * bulletData.Forward;
            bullet.TotalTraveledDistance = 0f;
            bullet.Score = 0;
            bullet.IsPlayerBullet = bulletData.IsPlayerBullet;
            Logger.Log($"Spawned bullet at {bullet.Position} with velocity {bullet.LinearVelocity}, forward {bulletData.Forward}");

            return bullet;
        }

        void HandleDestroyedBullets(ref PlayerState playerState, List<IBullet> existingBullets)
        {
            List<IBullet> bulletsToRemove = new();
            foreach (var bullet in existingBullets)
            {
                if (bullet.IsDestroyed)
                {
                    if (bullet.IsPlayerBullet && !playerState.GameOver)
                    {
                        playerState.Score += bullet.Score;
                    }
                    bulletsToRemove.Add(bullet);
                }
            }

            foreach (var bullet in bulletsToRemove)
            {
                bulletsPool.Add(bullet);
                existingBullets.Remove(bullet);
            }
        }
    }
}
