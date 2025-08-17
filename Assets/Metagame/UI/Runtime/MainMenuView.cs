using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Metagame.UI.Runtime
{
    public class MainMenuView : MonoBehaviour
    {
        bool clickedStartGame;

        public async Task WaitForCompletionAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && !clickedStartGame)
            {
                await Task.Yield();
            }
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