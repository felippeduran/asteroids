using System;
using UnityEngine;

namespace Gameplay.UI.Runtime
{
    public class GameOverView : MonoBehaviour
    {
        public event Action OnExitGame = delegate { };

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ExitGame();
            }
        }

        public void ExitGame()
        {
            OnExitGame.Invoke();
        }
    }
}