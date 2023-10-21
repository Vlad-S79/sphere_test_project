using UnityEngine;
using Zenject;

namespace Core.Camera
{
    public partial class CameraComponent
    {
        private CameraBehaviour _cameraBehaviour;
        
        private UnityEngine.Camera _camera;
        private Transform _cameraTransform;

        private ICameraContainer _cameraContainer;

        [Inject]
        public void Init(TickableManager tickableManager)
        {
            var camera = new GameObject("camera");
            _cameraTransform = camera.GetComponent<Transform>();
            _camera = camera.AddComponent<UnityEngine.Camera>();
            
            // _cameraBehaviour = camera.AddComponent<CameraBehaviour>();
            _cameraBehaviour = new CameraBehaviour(_camera, _cameraTransform);
            tickableManager.AddLate(_cameraBehaviour);

            InitShakeData();
            InitInterpolationData();
        }

        public UnityEngine.Camera GetCamera() => _camera;
        
        public void SetCameraContainer(ICameraContainer cameraContainer)
        {
            if(_cameraContainer == cameraContainer) return;
            
            _cameraBehaviour.SetCameraContainer(cameraContainer);
            _cameraContainer = cameraContainer;
        }

        public void AddCameraEffect(ICameraContainer cameraContainerEffect) 
            => _cameraBehaviour.AddCameraEffect(cameraContainerEffect); 

        public void RemoveCameraEffect(ICameraContainer cameraContainerEffect)
            => _cameraBehaviour.RemoveCameraEffect(cameraContainerEffect);

        // public void UpdateCameraData() => _cameraBehaviour.UpdateCameraData();
        // public void UpdateCamera() => _cameraBehaviour.UpdateCamera();

        public CameraData GetCameraData() => _cameraBehaviour.GetCameraData();
        public ICameraContainer GetCameraContainer() => _cameraContainer;
        
        public void Dispose() { }
    }
}