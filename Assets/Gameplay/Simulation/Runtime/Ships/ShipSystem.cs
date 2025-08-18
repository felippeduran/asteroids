using UnityEngine;
using Random = Company.Utilities.Runtime.Random;
using Logger = Company.Utilities.Runtime.Logger;
using System;

namespace Gameplay.Simulation.Runtime
{
    public class ShipSystem
    {
        readonly IInputProvider inputProvider;

        public ShipSystem(IInputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        public FireBulletData[] UpdateShip(float deltaTime, IShip ship, ref PlayerState playerState, ShipConfig shipConfig, Bounds worldBounds)
        {
            ship.AngularVelocity = 0;

            var bulletsFired = new FireBulletData[] { };
            if (!playerState.GameOver && !playerState.Reviving)
            {
                bulletsFired = HandleShipInput(inputProvider.GetPlayerInput(), ship, worldBounds, shipConfig);
            }

            HandleShipFireCooldown(deltaTime, ship);
            HandleShipReloadAmmo(deltaTime, ship, shipConfig);
            HandleShipTeleportCooldown(deltaTime, ship, worldBounds);

            HandleDestroyedShip(deltaTime, ship, ref playerState, shipConfig);
            return bulletsFired;
        }

        FireBulletData[] HandleShipInput(PlayerInput input, IShip ship, Bounds worldBounds, ShipConfig shipConfig)
        {
            var fireBulletData = new FireBulletData[] { };

            if (input.TurnLeft)
            {
                ship.AngularVelocity = shipConfig.TurnSpeed;
            }

            if (input.TurnRight)
            {
                ship.AngularVelocity = -shipConfig.TurnSpeed;
            }

            if (input.Thrust)
            {
                ship.ThrustForce = shipConfig.ThrustForce * ship.Forward;
            }

            if (input.Fire)
            {
                if (ship.Ammo > 0 && ship.FireCooldown <= 0)
                {
                    ship.Ammo--;
                    ship.FireCooldown = 1f / shipConfig.FireRate;
                    fireBulletData = new FireBulletData[] { CreateFireBulletData(ship) };
                }
            }

            if (input.Teleport)
            {
                ship.IsTeleporting = true;
                ship.TeleportCooldown = shipConfig.TeleportTime;
                ship.Disable();
            }

            return fireBulletData;
        }

        Vector2 GetRandomPositionInsideBounds(Bounds worldBounds)
        {
            var random = new Random();
            return new Vector2(random.NextFloat() * worldBounds.size.x, random.NextFloat() * worldBounds.size.y) - new Vector2(worldBounds.size.x, worldBounds.size.y) / 2;
        }


        FireBulletData CreateFireBulletData(IShip ship)
        {
            return new FireBulletData
            {
                Position = ship.BulletSpawnPosition,
                Forward = ship.Forward,
                IsPlayerBullet = true
            };
        }

        void HandleShipTeleportCooldown(float deltaTime, IShip ship, Bounds worldBounds)
        {
            if (ship.IsTeleporting)
            {
                ship.TeleportCooldown = Math.Max(0f, ship.TeleportCooldown - deltaTime);
                if (ship.TeleportCooldown <= 0)
                {
                    ship.IsTeleporting = false;
                    ship.Position = GetRandomPositionInsideBounds(worldBounds);
                    ship.Enable();
                }
            }
        }

        void HandleShipFireCooldown(float deltaTime, IShip ship)
        {
            ship.FireCooldown = Math.Max(0f, ship.FireCooldown - deltaTime);
        }

        void HandleShipReloadAmmo(float deltaTime, IShip ship, ShipConfig shipConfig)
        {
            ship.AmmoReloadCooldown = Math.Max(0f, ship.AmmoReloadCooldown - deltaTime);
            if (ship.AmmoReloadCooldown <= 0)
            {
                ship.Ammo = Math.Min(ship.Ammo + 1, shipConfig.MaxAmmo);
                ship.AmmoReloadCooldown = 1f / shipConfig.AmmoReloadRate;
            }
        }

        void HandleDestroyedShip(float deltaTime, IShip ship, ref PlayerState playerState, ShipConfig shipConfig)
        {
            if (playerState.Reviving)
            {
                playerState.ReviveCooldown -= deltaTime;

                if (playerState.ReviveCooldown < 0f)
                {
                    Logger.Log("Revive!");
                    // Note: In the original, the ship could be destroyed again while reviving if there was an asteroid/saucer in the way.
                    // Therefore we can also accept this to simplify things.
                    playerState.Reviving = false;
                    ReviveShip(ship, shipConfig);
                }
            }

            if (ship.IsDestroyed)
            {
                ship.Disable();

                if (!playerState.Reviving && !playerState.GameOver)
                {
                    Logger.Log("Ship destroyed");
                    playerState.Lives--;
                    if (playerState.Lives > 0)
                    {
                        Logger.Log("Reviving ship");
                        playerState.Reviving = true;
                        playerState.ReviveCooldown = shipConfig.ReviveCooldown;
                    }
                }
            }
        }
        
        void ReviveShip(IShip ship, ShipConfig shipConfig)
        {
            ship.Position = new Vector2(0, 0);
            ship.Forward = Vector2.up;
            ship.Ammo = shipConfig.MaxAmmo;
            ship.AmmoReloadCooldown = 0f;
            ship.FireCooldown = 0f;
            ship.IsDestroyed = false;
            ship.Enable();
        }
    }
}
