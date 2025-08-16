namespace Gameplay.Simulation.Runtime
{
    public interface IInputProvider
    {
        PlayerInput GetPlayerInput();
    }

    public struct PlayerInput
    {
        public bool TurnLeft;
        public bool TurnRight;
        public bool Thrust;
        public bool Fire;
        public bool Teleport;
    }
}