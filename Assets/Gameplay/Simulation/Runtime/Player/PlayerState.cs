using System;

[Serializable]
public struct PlayerState
{
    public int Score;
    public int Lives;
    public int NextLifeScore;
    public bool Reviving;
    public float ReviveCooldown;
    public readonly bool GameOver => Lives <= 0;
}