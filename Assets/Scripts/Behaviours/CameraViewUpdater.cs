using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class CameraViewUpdater : MonoBehaviour
    {
        public EntityInstance viewBoundsInstance;
        public float offset = 1.0f;

        [SerializeField]
        protected Camera _camera;

        public float Width
        {
            get => _camera.aspect * Height;
        }

        public float Height
        {
            get => _camera.orthographicSize * 2.0f;
        }

        public float2 Size
        {
            get => new float2(Width, Height);
        }

        public float2 Min
        {
            get => -Size * 0.5f;
        }

        public float2 Max
        {
            get => Size * 0.5f;
        }

        private void Update()
        {
            var bounds = new Bounds
            {
                min = Min - offset,
                max = Max + offset
            };

            viewBoundsInstance.TrySetData(bounds);
        }
    }
}
