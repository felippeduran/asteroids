using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Metagame.Presentation.Runtime;

namespace Metagame.UI.Runtime
{
    public class MainMenuView : MonoBehaviour, IMainMenuView
    {
        bool clickedStartGame;

        public async Task<bool> WaitForCompletionAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && !clickedStartGame)
            {
                clickedStartGame = false;
                await Task.Yield();
            }

            return clickedStartGame;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
        }

        public void StartGame()
        {
            clickedStartGame = true;
        }
    }
}