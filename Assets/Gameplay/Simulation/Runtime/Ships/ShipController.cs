using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    public class ShipController
    {
        readonly IInputProvider inputProvider;

        public ShipController(IInputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        public FireBulletData[] UpdateShip(IShip ship, ref PlayerState playerState, GameConfig gameConfig, Bounds worldBounds)
        {
            ship.AngularVelocity = 0;

            var bulletsFired = new FireBulletData[] { };
            if (!playerState.GameOver && !playerState.Reviving)
            {
                bulletsFired = HandleShipInput(inputProvider.GetPlayerInput(), ship, worldBounds, gameConfig.Ship);
            }

            HandleDestroyedShip(Time.deltaTime, ship, ref playerState, gameConfig);
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
                fireBulletData = new FireBulletData[] { CreateFireBulletData(ship) };
            }

            if (input.Teleport)
            {
                // TODO: Add a teleport delay
                ship.Position = GetRandomPositionInsideBounds(worldBounds);
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

        void HandleDestroyedShip(float deltaTime, IShip ship, ref PlayerState playerState, GameConfig gameConfig)
        {
            if (ship.IsDestroyed)
            {
                ship.Disable();

                if (!playerState.Reviving && !playerState.GameOver)
                {
                    Debug.Log("Ship destroyed");
                    playerState.Lives--;
                    if (playerState.Lives > 0)
                    {
                        Debug.Log("Reviving ship");
                        playerState.Reviving = true;
                        playerState.ReviveCooldown = gameConfig.ReviveCooldown;
                    }
                }
            }

            if (playerState.Reviving)
            {
                playerState.ReviveCooldown -= deltaTime;

                if (playerState.ReviveCooldown < 0f)
                {
                    Debug.Log("Revive!");
                    // TODO: Watch out for the ship being destroyed again while reviving
                    playerState.Reviving = false;
                    ReviveShip(ship);
                }
            }
        }
        
        void ReviveShip(IShip ship)
        {
            ship.Position = new Vector2(0, 0);
            ship.IsDestroyed = false;
            ship.Enable();
        }
    }
}
