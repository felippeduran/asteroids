using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    [CreateAssetMenu(fileName = "GameConfigAsset", menuName = "Gameplay/GameConfigAsset")]
    public class GameConfigAsset : ScriptableObject
    {
        public GameConfig GameConfig;
    }
}