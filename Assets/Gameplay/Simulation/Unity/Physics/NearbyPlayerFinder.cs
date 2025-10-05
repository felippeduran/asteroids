using UnityEngine;
using Gameplay.Simulation.Runtime;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Unity
{
    public class NearbyPlayerFinder : INearbyPlayerFinder
    {
        readonly RaycastHit2D[] raycasHits = new RaycastHit2D[5];

        public bool FindNearbyPlayer(System.Numerics.Vector2 origin, float radius, System.Numerics.Vector2 direction, out NearbyPlayer nearbyPlayer)
        {
            var filter = new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = true,
                layerMask = LayerMask.GetMask("Player")
            };
            var count = Physics2D.CircleCast(new Vector2(origin.X, origin.Y), radius, new Vector2(direction.X, direction.Y), filter, raycasHits, 0f);
            if (count > 0)
            {
                Logger.Log($"Found {count} targets");
                var hitTransform = raycasHits[0].rigidbody.transform;
                nearbyPlayer = new NearbyPlayer { Position = new System.Numerics.Vector2(hitTransform.position.x, hitTransform.position.y) };
                return true;
            }
            nearbyPlayer = new NearbyPlayer { Position = System.Numerics.Vector2.Zero };
            return false;
        }
    }
}