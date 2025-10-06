using System;
using UnityEngine;
using Gameplay.Simulation.Runtime;
using Company.Utilities.Runtime.Unity;

namespace Gameplay.UI.Runtime
{
    [ExecuteInEditMode]
    public class CameraGroup : MonoBehaviour, ICameraGroup, IDisposable
    {
        [SerializeField] private Camera Center;
        [SerializeField] private Camera Top;
        [SerializeField] private Camera Bottom;
        [SerializeField] private Camera Left;
        [SerializeField] private Camera Right;

        Vector2 lastScreenSize;

        Camera[] AllCameras => new Camera[] { Center, Top, Bottom, Left, Right };

        public void SetWorldSize(System.Numerics.Vector2 worldSize)
        {
            foreach (var camera in AllCameras)
            {
                camera.orthographicSize = worldSize.Y / 2f;
                camera.aspect = worldSize.X / worldSize.Y;
            }
            UpdateCameraPositions(worldSize.ToUnity());
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        Vector2 GetWorldSize()
        {
            float height = Center.orthographicSize * 2f;
            float width = height * Center.aspect;
            return new Vector2(width, height);
        }

        void Update()
        {
            var screenSize = new Vector2(Screen.width, Screen.height);
            if (screenSize == lastScreenSize)
            {
                return;
            }

            lastScreenSize = screenSize;

            if (Center == null || Top == null || Bottom == null || Left == null || Right == null)
            {
                return;
            }

            var worldSize = GetWorldSize();
            UpdateCameraPositions(worldSize);
        }
        
        void UpdateCameraPositions(Vector2 worldSize)
        {
            Top.transform.position = Center.transform.position + new Vector3(0, worldSize.y, 0);
            Bottom.transform.position = Center.transform.position + new Vector3(0, -worldSize.y, 0);
            Left.transform.position = Center.transform.position + new Vector3(-worldSize.x, 0, 0);
            Right.transform.position = Center.transform.position + new Vector3(worldSize.x, 0, 0);
        }
    }
}