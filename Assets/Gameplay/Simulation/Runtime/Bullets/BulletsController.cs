using System.Collections.Generic;
using UnityEngine;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime
{
    public struct FireBulletData
    {
        public Vector2 Position;
        public Vector2 Forward;
        public bool IsPlayerBullet;
    }

    public class BulletsController
    {
        readonly ObjectPool<Bullet> bulletsPool;

        public BulletsController(ObjectPool<Bullet> bulletsPool)
        {
            this.bulletsPool = bulletsPool;
        }

        public void UpdateBullets(float deltaTime, FireBulletData[] bulletsFired, ref PlayerState playerState, List<Bullet> existingBullets, BulletsConfig bulletsConfig)
        {
            foreach (var bulletData in bulletsFired)
            {
                var bullet = SpawnBullet(bulletData, bulletsConfig);
                existingBullets.Add(bullet);
            }

            foreach (var bullet in existingBullets)
            {
                if (!bullet.IsDestroyed)
                {
                    bullet.TotalTraveledDistance += bullet.LinearVelocity.magnitude * deltaTime;
                    if (bullet.TotalTraveledDistance > bulletsConfig.TravelDistance)
                    {
                        bullet.IsDestroyed = true;
                    }
                }
            }

            HandleDestroyedBullets(ref playerState, existingBullets);
        }

        Bullet SpawnBullet(FireBulletData bulletData, BulletsConfig bulletsConfig)
        {
            Bullet bullet = bulletsPool.Get();

            bullet.IsDestroyed = false;
            bullet.Position = bulletData.Position;
            bullet.LinearVelocity = bulletsConfig.Speed * bulletData.Forward;
            bullet.TotalTraveledDistance = 0f;
            bullet.Score = 0;
            bullet.IsPlayerBullet = bulletData.IsPlayerBullet;
            Logger.Log($"Spawned bullet at {bullet.Position} with velocity {bullet.LinearVelocity}, forward {bulletData.Forward}");

            return bullet;
        }

        void HandleDestroyedBullets(ref PlayerState playerState, List<Bullet> existingBullets)
        {
            List<Bullet> bulletsToRemove = new();
            foreach (var bullet in existingBullets)
            {
                if (bullet.IsDestroyed)
                {
                    playerState.Score += bullet.Score;
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
