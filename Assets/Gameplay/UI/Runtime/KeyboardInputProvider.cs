using UnityEngine;
using Gameplay.Simulation.Runtime;

namespace Gameplay.UI.Runtime
{
    public class KeyboardInputProvider : IInputProvider
    {
        public PlayerInput GetPlayerInput()
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
    }
}