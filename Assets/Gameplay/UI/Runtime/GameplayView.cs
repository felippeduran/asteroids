using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gameplay.Presentation.Runtime;

namespace Gameplay.UI.Runtime
{
    public class GameplayView : MonoBehaviour, IGameplayView
    {
        [SerializeField] GameObject lifePrefab;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] GameObject livesRoot;
        [SerializeField] List<GameObject> lives;

        public void UpdateUI(int lives, int score)
        {
            UpdateLives(lives);

            scoreText.text = score.ToString();
        }

        public InputData GetInput()
        {
            return new InputData
            {
                TurnLeft = Input.GetKey(KeyCode.A),
                TurnRight = Input.GetKey(KeyCode.D),
                Thrust = Input.GetKey(KeyCode.W),
                Fire = Input.GetKeyDown(KeyCode.Space),
                Teleport = Input.GetKeyDown(KeyCode.M)
            };
        }

        void UpdateLives(int remainingLives)
        {
            var missingLives = remainingLives - lives.Count;
            for (int i = 0; i < missingLives; i++)
            {
                var life = Instantiate(lifePrefab, livesRoot.transform);
                lives.Add(life);
            }

            var livesToRemove = lives.Count - remainingLives;
            for (int i = 0; i < livesToRemove; i++)
            {
                Destroy(lives[i].gameObject);
                lives.RemoveAt(i);
            }
        }
    }
}