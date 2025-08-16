using UnityEngine;
using Gameplay.Simulation.Runtime;

namespace Application.Runtime
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] GameConfig gameConfig;
        [SerializeField] GameplayAssets gameplayAssets;

        void Start()
        {
            GameplayBootstrap gameplay = new GameplayFactory(gameplayAssets, gameConfig).Create();
        }
    }
}