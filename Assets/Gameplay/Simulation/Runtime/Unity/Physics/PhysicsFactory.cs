using Gameplay.Simulation.Runtime;

namespace Gameplay.Simulation.Runtime.Unity
{
    public class PhysicsFactory : IPhysicsFactory
    {
        public INearbyPlayerFinder CreateNearbyPlayerFinder()
        {
            return new NearbyPlayerFinder();
        }
    }
}