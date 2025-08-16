using UnityEngine;

public class Random
{
    readonly System.Random random;

    public Random()
    {
        random = new System.Random();
    }

    public float Range(float min, float max)
    {
        return NextFloat() * (max - min) + min;
    }

    public float NextFloat()
    {
        return (float)random.NextDouble();
    }

    public Vector2 NextDirection()
    {
        return (new Vector2((float)random.NextDouble(), (float)random.NextDouble()) - 0.5f * Vector2.one).normalized;
    }

    public Vector2 InsideUnitCircle()
    {
        return NextFloat() * NextDirection();
    }
}