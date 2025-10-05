using UnityEngine;
using Gameplay.Simulation.Runtime;

namespace Gameplay.Simulation.Unity
{
    [CreateAssetMenu(fileName = "GameConfigAsset", menuName = "Gameplay/GameConfigAsset")]
    public class GameConfigAsset : ScriptableObject
    {
        public GameConfig GameConfig;
    }
}