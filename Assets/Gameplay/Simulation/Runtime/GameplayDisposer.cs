using System;
using UnityEngine;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime
{
    public class GameplayDisposer : IDisposable
    {
        readonly Ship playerShip;
        readonly GameObject gameLoopObject;
        readonly IDisposable[] objectPools;

        public GameplayDisposer(Ship playerShip, GameObject gameLoopObject, IDisposable[] objectPools)
        {
            this.playerShip = playerShip;
            this.gameLoopObject = gameLoopObject;
            this.objectPools = objectPools;
        }

        public void Dispose()
        {
            Logger.Log("Disposing gameplay");
            foreach (var pool in objectPools)
            {
                pool.Dispose();
            }
            GameObject.Destroy(playerShip.gameObject);
            GameObject.Destroy(gameLoopObject);
        }
    }
}