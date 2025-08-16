using System;

[Serializable]
public struct AsteroidWaveState
{
    public int Current;
    public bool NextWave;
    public float NextWaveCooldown;
}