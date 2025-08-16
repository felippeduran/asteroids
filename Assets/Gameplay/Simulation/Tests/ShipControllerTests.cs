using NUnit.Framework;
using UnityEngine;
using Gameplay.Simulation.Runtime;

namespace Gameplay.Simulation.Tests
{
    [TestFixture]
    public class ShipControllerTests
    {
        const float kDeltaTime = 0.016f;

        [Test]
        public void UpdateShip_WithNoInput_ShouldResetAngularVelocity()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { AngularVelocity = 45f };
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput();

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(0f, fakeShip.AngularVelocity);
        }

        [Test]
        public void UpdateShip_WithTurnLeftInput_ShouldSetPositiveAngularVelocity()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { };
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput { TurnLeft = true };

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(gameConfig.Ship.TurnSpeed, fakeShip.AngularVelocity);
        }

        [Test]
        public void UpdateShip_WithTurnRightInput_ShouldSetNegativeAngularVelocity()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { };
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput { TurnRight = true };

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(-gameConfig.Ship.TurnSpeed, fakeShip.AngularVelocity);
        }

        [Test]
        public void UpdateShip_WithThrustInput_ShouldApplyThrustForce()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { Forward = Vector2.up };
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput { Thrust = true };

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(gameConfig.Ship.ThrustForce * Vector2.up, fakeShip.ThrustForce);
        }

        [Test]
        public void UpdateShip_WithFireInput_ShouldReturnBulletData()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip
            { 
                Position = Vector2.one,
                Forward = Vector2.up
            };
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput { Fire = true };

            // Act
            var bulletsFired = CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(1, bulletsFired.Length);
            Assert.AreEqual(fakeShip.BulletSpawnPosition, bulletsFired[0].Position);
            Assert.AreEqual(fakeShip.Forward, bulletsFired[0].Forward);
            Assert.IsTrue(bulletsFired[0].IsPlayerBullet);
        }

        [Test]
        public void UpdateShip_WithTeleportInput_ShouldMoveShipToRandomPosition()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var originalPosition = new Vector2(1f, 1f);
            var fakeShip = new FakeShip { Position = originalPosition };
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput { Teleport = true };

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreNotEqual(originalPosition, fakeShip.Position);
            Assert.IsTrue(worldBounds.Contains(fakeShip.Position));
        }

        [Test]
        public void UpdateShip_WithMultipleInputs_ShouldHandleAllInputs()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { Forward = Vector2.up };
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput 
            { 
                TurnLeft = true, 
                Thrust = true, 
                Fire = true 
            };

            // Act
            var bulletsFired = CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(gameConfig.Ship.TurnSpeed, fakeShip.AngularVelocity);
            Assert.AreEqual(gameConfig.Ship.ThrustForce * Vector2.up, fakeShip.ThrustForce);
            Assert.AreEqual(1, bulletsFired.Length);
        }

        [Test]
        public void UpdateShip_WhenGameOver_ShouldNotProcessInput()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { };
            var playerState = new PlayerState { Lives = 0 };
            var input = new PlayerInput { Fire = true };

            // Act
            var bulletsFired = CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(0, bulletsFired.Length);
        }

        [Test]
        public void UpdateShip_WhenReviving_ShouldNotProcessInput()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { IsDestroyed = true };    
            var playerState = new PlayerState { Lives = 3, Reviving = true };
            var input = new PlayerInput { Fire = true };

            // Act
            var bulletsFired = CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(0, bulletsFired.Length);
        }

        [Test]
        public void UpdateShip_WhenShipDestroyed_ShouldDecreaseLivesAndStartReviving()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { IsDestroyed = true };
            int initialLives = 3;
            var playerState = new PlayerState { Lives = initialLives };
            var input = new PlayerInput();

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(initialLives - 1, playerState.Lives);
            Assert.IsTrue(playerState.Reviving);
            Assert.AreEqual(gameConfig.ReviveCooldown, playerState.ReviveCooldown);
            Assert.IsFalse(fakeShip.IsActive);
        }

        [Test]
        public void UpdateShip_WhenRevivingAndCooldownExpired_ShouldReviveShip()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { IsDestroyed = true, IsActive = false };
            var playerState = new PlayerState { Reviving = true, ReviveCooldown = 0f };
            var input = new PlayerInput();

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.IsFalse(playerState.Reviving);
            Assert.IsFalse(fakeShip.IsDestroyed);
            Assert.IsTrue(fakeShip.IsActive);
            Assert.AreEqual(Vector2.zero, fakeShip.Position);
            Assert.AreEqual(Vector2.up, fakeShip.Forward);
        }

        [Test]
        public void UpdateShip_WhenRevivingAndCooldownNotExpired_ShouldContinueReviving()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { IsDestroyed = true, IsActive = false };
            var initialReviveCooldown = 1f;
            var playerState = new PlayerState { Reviving = true, ReviveCooldown = initialReviveCooldown };
            var input = new PlayerInput();

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.IsTrue(playerState.Reviving);
            Assert.That(playerState.ReviveCooldown, Is.LessThan(initialReviveCooldown));
        }

        [Test]
        public void UpdateShip_WhenGameOverAndShipDestroyed_ShouldNotDecreaseLives()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { IsDestroyed = true };
            int initialLives = 0;
            var playerState = new PlayerState { Lives = initialLives };
            var input = new PlayerInput();

            // Act
            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            // Assert
            Assert.AreEqual(initialLives, playerState.Lives);
            Assert.IsFalse(playerState.Reviving);
        }

        [Test]
        public void UpdateShip_WhenRevivingAndShipDestroyed_ShouldNotDecreaseLives()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { IsDestroyed = true };
            int initialLives = 3;
            var playerState = new PlayerState { Lives = initialLives, Reviving = true, ReviveCooldown = 0.1f };
            var input = new PlayerInput();


            CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            Assert.AreEqual(initialLives, playerState.Lives);
            Assert.IsTrue(playerState.Reviving);
        }

        [Test]
        public void UpdateShip_WithNoInput_ShouldReturnEmptyBulletArray()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip { };
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput();


            var bulletsFired = CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            Assert.AreEqual(0, bulletsFired.Length);
        }

        [Test]
        public void UpdateShip_WithFireInput_ShouldUseCorrectBulletSpawnPosition()
        {
            var gameConfig = CreateGameConfig();
            var worldBounds = TestUtilities.CreateWorldBounds();
            var fakeShip = new FakeShip
            {
                Position = new Vector2(5f, 3f),
                Forward = new Vector2(1f, 0f)
            };            
            var playerState = new PlayerState { Lives = 3 };
            var input = new PlayerInput { Fire = true };

            var bulletsFired = CreateShipController(input).UpdateShip(kDeltaTime, fakeShip, ref playerState, gameConfig, worldBounds);

            Assert.AreEqual(fakeShip.BulletSpawnPosition, bulletsFired[0].Position);
        }

        ShipController CreateShipController(PlayerInput input)
        {
            var fakeInputProvider = new FakeInputProvider(input);
            return new ShipController(fakeInputProvider);
        }

        GameConfig CreateGameConfig()
        {
            return new GameConfig
            {
                Ship = new ShipConfig
                {
                    ThrustForce = 10f,
                    TurnSpeed = 90f
                },
                ReviveCooldown = 2f
            };
        }

        class FakeInputProvider : IInputProvider
        {
            readonly PlayerInput currentInput;

            public FakeInputProvider(PlayerInput input)
            {
                currentInput = input;
            }

            public PlayerInput GetPlayerInput()
            {
                return currentInput;
            }
        }

        class FakeShip : IShip
        {
            public Vector2 Position { get; set; }
            public Vector2 Forward { get; set; }
            public bool IsDestroyed { get; set; }
            public bool IsTeamPlayer { get; set; }
            public Vector2 ThrustForce { get; set; }
            public float AngularVelocity { get; set; }
            public bool IsThrusting => ThrustForce.magnitude > Mathf.Epsilon;
            public Vector2 BulletSpawnPosition => Position + Forward;
            public bool IsActive { get; set; } = true;

            public void Disable()
            {
                IsActive = false;
            }

            public void Enable()
            {
                IsActive = true;
            }
        }
    }
}
