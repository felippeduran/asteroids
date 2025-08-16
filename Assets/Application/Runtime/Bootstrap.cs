using UnityEngine;
using Gameplay.Simulation.Runtime;
using Gameplay.UI;

namespace Application.Runtime
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] GameConfig gameConfig;
        [SerializeField] GameplayAssets gameplayAssets;

        void Start()
        {
            var inputProvider = new KeyboardInputProvider();
            GameplayBootstrap gameplay = new GameplayFactory(gameplayAssets, gameConfig).Create(inputProvider);
        }
    }
}