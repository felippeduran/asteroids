using UnityEngine;

namespace Metagame.UI.Runtime
{
    [CreateAssetMenu(fileName = "MetagameViewLibrary", menuName = "Metagame/MetagameViewLibrary")]
    public class MetagameViewLibrary : ScriptableObject
    {
        public MainMenuView MainMenuViewPrefab;
    }
}