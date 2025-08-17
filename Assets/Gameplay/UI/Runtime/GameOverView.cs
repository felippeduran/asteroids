using System;
using System.Threading;
using System.Threading.Tasks;
using Company.Utilities.Runtime;
using UnityEngine;

namespace Gameplay.UI.Runtime
{
    public class GameOverView : View
    {
        bool exit = false;

        public async Task WaitForCompletionAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && !exit)
            {
                await Task.Yield();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ExitGame();
            }
        }

        public void ExitGame()
        {
            exit = true;
        }
    }
}