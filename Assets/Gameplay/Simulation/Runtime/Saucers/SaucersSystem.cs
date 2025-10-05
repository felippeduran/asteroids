using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Company.Utilities.Runtime;
using Random = Company.Utilities.Runtime.Random;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime
{
    public struct NearbyPlayer
    {
        public Vector2 Position;
    }

    public interface INearbyPlayerFinder
    {
        bool FindNearbyPlayer(Vector2 origin, float radius, Vector2 direction, out NearbyPlayer nearbyPlayer);
    }

    public class SaucersSystem
    {
        readonly INearbyPlayerFinder nearbyPlayerFinder;
        readonly IObjectPool<ISaucer> saucersPool;
        readonly Random random;

        public SaucersSystem(INearbyPlayerFinder nearbyPlayerFinder, IObjectPool<ISaucer> saucersPool)
        {
            this.nearbyPlayerFinder = nearbyPlayerFinder;
            this.saucersPool = saucersPool;
            this.random = new Random();
        }

        public FireBulletData[] UpdateSaucers(float deltaTime, ref SaucersState saucerState, List<ISaucer> existingSaucers, SaucersConfig saucersConfig, Bounds worldBounds)
        {
            HandleSaucerSpawning(deltaTime, ref saucerState, existingSaucers, saucersConfig, worldBounds);
            HandleSaucerMovement(deltaTime, existingSaucers, saucersConfig);
            var firedBullets = HandleSaucerShooting(deltaTime, existingSaucers, saucersConfig);
            HandleSaucerBoundaryCrossing(existingSaucers, worldBounds);
            HandleDestroyedSaucers(existingSaucers);
            return firedBullets;
        }

        void HandleSaucerSpawning(float deltaTime, ref SaucersState saucersState, List<ISaucer> existingSaucers, SaucersConfig saucersConfig, Bounds worldBounds)
        {
            saucersState.SpawnCooldown -= deltaTime;
            if (saucersState.SpawnCooldown < 0f)
            {
                Logger.Log("Try spawning saucers");
                saucersState.SpawnCooldown = saucersConfig.SpawnCooldown;
                foreach (var spawnRate in saucersConfig.SpawnConfigs)
                {
                    if (!existingSaucers.Any(x => x.Type == spawnRate.Type) && random.NextFloat() < spawnRate.Chance)
                    {
                        var saucer = SpawnSaucer(saucersConfig.Saucers.GetSaucerConfigFor(spawnRate.Type), worldBounds);
                        existingSaucers.Add(saucer);
                    }
                }
            }
        }

        FireBulletData[] HandleSaucerShooting(float deltaTime, List<ISaucer> existingSaucers, SaucersConfig saucersConfig)
        {
            var bulletsFired = new List<FireBulletData>();
            foreach (var saucer in existingSaucers)
            {
                saucer.ShootCooldown -= deltaTime;
                var saucerConfig = saucersConfig.Saucers.GetSaucerConfigFor(saucer.Type);
                if (saucer.ShootCooldown < 0f)
                {
                    Logger.Log($"Try shoot {saucer.Type} saucer");
                    saucer.ShootCooldown = 1f / saucerConfig.Aim.FireRate;

                    if (nearbyPlayerFinder.FindNearbyPlayer(saucer.Position, saucerConfig.Aim.SightDistance, Vector2.One, out var nearbyPlayer))
                    {
                        var random = new Random();
                        var targetPosition = nearbyPlayer.Position;

                        var fireAnchorPosition = saucer.GunAnchorPosition;
                        var direction = Vector2.Normalize(targetPosition - fireAnchorPosition);

                        var finalDirection = random.GetRandomDirectionFromCone(direction, saucerConfig.Aim.FireAngle);
                        var startPosition = saucer.GetGunTipForDirection(finalDirection);
                        bulletsFired.Add(new FireBulletData
                        {
                            Position = startPosition,
                            Forward = finalDirection,
                            IsPlayerBullet = false
                        });
                    }
                }
            }
            return bulletsFired.ToArray();
        }

        void HandleSaucerMovement(float deltaTime, List<ISaucer> existingSaucers, SaucersConfig saucersConfig)
        {
            foreach (var saucer in existingSaucers)
            {
                saucer.TurnCooldown -= deltaTime;
                if (saucer.TurnCooldown < 0f)
                {
                    Logger.Log($"Try turn {saucer.Type} saucer");
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

                        saucer.LinearVelocity = Vector2.Normalize(new Vector2(System.Math.Sign(saucer.LinearVelocity.X), newVerticalComponent)) * saucerConfig.Movement.Speed;
                    }
                }
            }
        }

        void HandleSaucerBoundaryCrossing(List<ISaucer> existingSaucers, Bounds worldBounds)
        {
            var saucersToRemove = new List<ISaucer>();
            foreach (var saucer in existingSaucers)
            {
                if (saucer.LinearVelocity.X > 0 && saucer.Position.X > worldBounds.Max.X)
                {
                    Logger.Log($"Saucer {saucer.Type} crossed the screen on the right");
                    saucersToRemove.Add(saucer);
                }
                else if (saucer.LinearVelocity.X < 0 && saucer.Position.X < worldBounds.Min.X)
                {
                    Logger.Log($"Saucer {saucer.Type} crossed the screen on the left");
                    saucersToRemove.Add(saucer);
                }
            }

            foreach (var saucer in saucersToRemove)
            {
                saucersPool.Add(saucer);
                existingSaucers.Remove(saucer);
            }
        }

        void HandleDestroyedSaucers(List<ISaucer> existingSaucers)
        {
            var saucersToRemove = new List<ISaucer>();
            foreach (var saucer in existingSaucers)
            {
                if (saucer.IsDestroyed)
                {
                    saucersToRemove.Add(saucer);
                }
            }

            foreach (var saucer in saucersToRemove)
            {
                saucersPool.Add(saucer);
                existingSaucers.Remove(saucer);
            }
        }

        ISaucer SpawnSaucer(SaucerConfig saucerConfig, Bounds worldBounds)
        {
            var saucer = saucersPool.Get();

            saucer.Type = saucerConfig.Type;

            var direction = random.NextFloat() > 0.5f ? 1 : -1;
            saucer.Position = new Vector2(direction < 0f ? worldBounds.Size.X : 0, random.NextFloat() * worldBounds.Size.Y) - new Vector2(worldBounds.Size.X, worldBounds.Size.Y) / 2;
            saucer.LinearVelocity = new Vector2(direction, 0) * saucerConfig.Movement.Speed;
            saucer.TurnCooldown = random.NextFloat() * saucerConfig.Movement.TurnCooldown;
            saucer.IsDestroyed = false;
            saucer.ShootCooldown = random.Range(0.5f, 1f) * 1f / saucerConfig.Aim.FireRate;

            return saucer;
        }
    }
}
