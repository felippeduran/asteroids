using System;
using System.Threading;
using System.Threading.Tasks;
using Company.Utilities.Runtime;
using UnityEngine;

namespace Metagame.UI.Runtime
{
    public class MainMenuView : View
    {
        bool clickedStartGame;

        public async Task<bool> WaitForCompletionAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && !clickedStartGame)
            {
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