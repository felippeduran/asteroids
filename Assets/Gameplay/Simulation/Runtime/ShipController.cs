using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    public struct PlayerInput
    {
        public bool TurnLeft;
        public bool TurnRight;
        public bool Thrust;
        public bool Fire;
        public bool Teleport;
    }

    public class ShipController
    {
        public ShipController(){ }

        public FireBulletData[] UpdateShip(Ship ship, ref PlayerState playerState, GameConfig gameConfig, Bounds worldBounds)
        {
            ship.Reset();

            var bulletsFired = new FireBulletData[] { };
            if (!playerState.GameOver && !playerState.Reviving)
            {
                bulletsFired = HandleShipInput(GetPlayerInput(), ship, worldBounds);
            }

            HandleDestroyedShip(Time.deltaTime, ship, ref playerState, gameConfig);
            return bulletsFired;
        }

        PlayerInput GetPlayerInput()
        {
            return new PlayerInput
            {
                TurnLeft = Input.GetKey(KeyCode.A),
                TurnRight = Input.GetKey(KeyCode.D),
                Thrust = Input.GetKey(KeyCode.W),
                Fire = Input.GetKeyDown(KeyCode.Space),
                Teleport = Input.GetKeyDown(KeyCode.M)
            };
        }

        FireBulletData[] HandleShipInput(PlayerInput input, Ship ship, Bounds worldBounds)
        {
            var fireBulletData = new FireBulletData[] { };

            if (input.TurnLeft)
            {
                ship.TurnLeft();
            }

            if (input.TurnRight)
            {
                ship.TurnRight();
            }

            if (input.Thrust)
            {
                ship.Thrust();
            }

            if (input.Fire)
            {
                // TODO: Fire a bullet
                fireBulletData = new FireBulletData[]{
                    new FireBulletData{
                        Position = ship.Position,
                        Forward = ship.Forward,
                        IsPlayerBullet = true
                    }
                };
            }

            if (input.Teleport)
            {
                // TODO: Teleport the ship to a random position
                var random = new Random();
                ship.Position = new Vector2(random.NextFloat() * worldBounds.size.x, random.NextFloat() * worldBounds.size.y) - new Vector2(worldBounds.size.x, worldBounds.size.y) / 2;
            }

            return fireBulletData;
        }

        void HandleDestroyedShip(float deltaTime, Ship ship, ref PlayerState playerState, GameConfig gameConfig)
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
                    ship.Position = new Vector2(0, 0);
                    ship.IsDestroyed = false;
                    ship.Enable();
                }
            }
        }
    }
}
