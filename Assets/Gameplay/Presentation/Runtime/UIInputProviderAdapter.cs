using Gameplay.Simulation.Runtime;

namespace Gameplay.Presentation.Runtime
{
    public class UIInputProviderAdapter : IInputProvider
    {
        readonly IGameplayView gameplayView;

        public UIInputProviderAdapter(IGameplayView gameplayView)
        {
            this.gameplayView = gameplayView;
        }

        public PlayerInput GetPlayerInput()
        {
            var inputData = gameplayView.GetInput();
            return new PlayerInput
            {
                TurnLeft = inputData.TurnLeft,
                TurnRight = inputData.TurnRight,
                Thrust = inputData.Thrust,
                Fire = inputData.Fire,
                Teleport = inputData.Teleport
            };
        }
    }
}