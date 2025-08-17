using UnityEngine;

namespace Gameplay.UI.Runtime
{
    [CreateAssetMenu(fileName = "GameplayViewLibrary", menuName = "Gameplay/GameplayViewLibrary")]
    public class GameplayViewLibrary : ScriptableObject
    {
        [SerializeField] CameraGroup cameras;
        [SerializeField] GameplayView gameplayViewPrefab;
        [SerializeField] GameOverView gameOverViewPrefab;

        public CameraGroup CreateCameraGroup()
        {
            return Instantiate(cameras);
        }

        public GameplayView CreateGameplayView()
        {
            var gameplayView = Instantiate(gameplayViewPrefab);
            gameplayView.gameObject.SetActive(true);
            return gameplayView;
        }

        public GameOverView CreateGameOverView()
        {
            var gameOverView = Instantiate(gameOverViewPrefab);
            gameOverView.gameObject.SetActive(true);
            return gameOverView;
        }
    }
}