using UnityEngine;

namespace Gameplay.UI.Runtime
{
    [CreateAssetMenu(fileName = "GameplayViewLibrary", menuName = "Gameplay/GameplayViewLibrary")]
    public class GameplayViewLibrary : ScriptableObject
    {
        public CameraGroup Cameras;
        public GameplayView GameplayViewPrefab;
        public GameOverView GameOverViewPrefab;
    }
}