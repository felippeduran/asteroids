using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Company.Utilities.Runtime;
using Random = Company.Utilities.Runtime.Random;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime
{
    public class SaucersSystem
    {
        readonly ObjectPool<Saucer> saucersPool;
        readonly Random random;
        readonly RaycastHit2D[] raycasHits = new RaycastHit2D[5];

        public SaucersSystem(ObjectPool<Saucer> saucersPool)
        {
            this.saucersPool = saucersPool;
            this.random = new Random();
        }

        public FireBulletData[] UpdateSaucers(float deltaTime, ref SaucersState saucerState, List<Saucer> existingSaucers, SaucersConfig saucersConfig, Bounds worldBounds)
        {
            HandleSaucerSpawning(deltaTime, ref saucerState, existingSaucers, saucersConfig, worldBounds);
            HandleSaucerMovement(deltaTime, existingSaucers, saucersConfig);
            var firedBullets = HandleSaucerShooting(deltaTime, existingSaucers, saucersConfig);
            HandleSaucerBoundaryCrossing(existingSaucers, worldBounds);
            HandleDestroyedSaucers(existingSaucers);
            return firedBullets;
        }

        void HandleSaucerSpawning(float deltaTime, ref SaucersState saucersState, List<Saucer> existingSaucers, SaucersConfig saucersConfig, Bounds worldBounds)
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

        FireBulletData[] HandleSaucerShooting(float deltaTime, List<Saucer> existingSaucers, SaucersConfig saucersConfig)
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

                    var filter = new ContactFilter2D {
                        useTriggers = true,
                        useLayerMask = true,
                        layerMask = LayerMask.GetMask("Player")
                    };
                    var count = Physics2D.CircleCast(saucer.Position, saucerConfig.Aim.SightDistance, Vector2.one, filter, raycasHits, 0f);
                    if (count > 0)
                    {
                        Logger.Log($"Found {count} targets");
                        var random = new Random();
                        var hit = raycasHits[random.Range(0, count)];
                        var targetPosition = hit.rigidbody.transform.position;

                        var fireAnchorPosition = saucer.ShootRing.Position;
                        var direction = (new Vector2(targetPosition.x, targetPosition.y) - fireAnchorPosition).normalized;

                        var finalDirection = random.GetRandomDirectionFromCone(direction, saucerConfig.Aim.FireAngle);
                        var startPosition = fireAnchorPosition + finalDirection * saucer.ShootRing.Radius;
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

        void HandleSaucerMovement(float deltaTime, List<Saucer> existingSaucers, SaucersConfig saucersConfig)
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

                        saucer.LinearVelocity = new Vector2(Mathf.Sign(saucer.LinearVelocity.x), newVerticalComponent).normalized * saucerConfig.Movement.Speed;
                    }
                }
            }
        }

        void HandleSaucerBoundaryCrossing(List<Saucer> existingSaucers, Bounds worldBounds)
        {
            var saucersToRemove = new List<Saucer>();
            foreach (var saucer in existingSaucers)
            {
                if (saucer.LinearVelocity.x > 0 && saucer.Position.x > worldBounds.max.x)
                {
                    Logger.Log($"Saucer {saucer.Type} crossed the screen on the right");
                    saucersToRemove.Add(saucer);
                }
                else if (saucer.LinearVelocity.x < 0 && saucer.Position.x < worldBounds.min.x)
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

        void HandleDestroyedSaucers(List<Saucer> existingSaucers)
        {
            var saucersToRemove = new List<Saucer>();
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

        Saucer SpawnSaucer(SaucerConfig saucerConfig, Bounds worldBounds)
        {
            var saucer = saucersPool.Get();

            saucer.Type = saucerConfig.Type;

            var direction = random.NextFloat() > 0.5f ? 1 : -1;
            saucer.Position = new Vector2(direction < 0f ? worldBounds.size.x : 0, random.NextFloat() * worldBounds.size.y) - new Vector2(worldBounds.size.x, worldBounds.size.y) / 2;
            saucer.LinearVelocity = new Vector2(direction, 0) * saucerConfig.Movement.Speed;
            saucer.TurnCooldown = random.NextFloat() * saucerConfig.Movement.TurnCooldown;
            saucer.IsDestroyed = false;
            saucer.ShootCooldown = random.Range(0.5f, 1f) * 1f / saucerConfig.Aim.FireRate;

            return saucer;
        }
    }
}
