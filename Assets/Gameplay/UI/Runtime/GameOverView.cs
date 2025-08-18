using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Gameplay.Presentation.Runtime;

namespace Gameplay.UI.Runtime
{
    public class GameOverView : MonoBehaviour, IGameOverView
    {
        [SerializeField] TMP_Text scoreText;

        bool exit = false;

        public async Task WaitForCompletionAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && !exit)
            {
                await Task.Yield();
            }
        }

        public void Setup(int score)
        {
            scoreText.text = score.ToString();
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