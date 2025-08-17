using UnityEngine;

namespace Metagame.UI.Runtime
{
    [CreateAssetMenu(fileName = "MetagameViewLibrary", menuName = "Metagame/MetagameViewLibrary")]
    public class MetagameViewLibrary : ScriptableObject
    {
        [SerializeField] MainMenuView mainMenuViewPrefab;

        public MainMenuView CreateMainMenuView()
        {
            var mainMenuView = Instantiate(mainMenuViewPrefab);
            mainMenuView.gameObject.SetActive(true);
            return mainMenuView;
        }
    }
}