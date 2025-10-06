using UnityEngine;
using Gameplay.Simulation.Runtime;

namespace Gameplay.Simulation.Runtime.Unity
{
    [CreateAssetMenu(fileName = "GameConfigAsset", menuName = "Gameplay/GameConfigAsset")]
    public class GameConfigAsset : ScriptableObject
    {
        public GameConfig GameConfig;
    }
}