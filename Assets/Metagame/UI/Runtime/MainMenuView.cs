using System;
using UnityEngine;

public class MainMenuView : MonoBehaviour
{
    public event Action OnStartGame = delegate { };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        OnStartGame.Invoke();
    }
}