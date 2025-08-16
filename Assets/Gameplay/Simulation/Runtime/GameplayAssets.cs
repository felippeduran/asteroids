using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    [CreateAssetMenu(fileName = "GameplayAssets", menuName = "Gameplay/GameplayAssets")]
    public class GameplayAssets : ScriptableObject
    {
        public Ship ShipPrefab;
        public Asteroid AsteroidPrefab;
        public Bullet BulletPrefab;
        public Saucer SaucerPrefab;
        public CameraGroup Cameras;
    }
}