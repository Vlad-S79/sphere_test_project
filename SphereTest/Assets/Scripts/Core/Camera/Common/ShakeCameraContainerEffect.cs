using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Camera.Common
{
    public class ShakeCameraContainer : ICameraContainer
    {
        private CameraData _cameraData;

        private float _durationSeconds;
        private float _magnitude;

        private CameraComponent _cameraComponent;

        private Vector3 _shakeVector = Vector3.zero;

        private float _currentTime;
        private AnimationCurve _animationCurve;

        public ShakeCameraContainer(CameraComponent cameraComponent)
        {
            _cameraComponent = cameraComponent;
            _animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
        }
        public void SetData(float durationSeconds, float magnitude)
        {
            _durationSeconds = durationSeconds;
            _magnitude = magnitude;
            _currentTime = 0;

            _shakeVector.x = Random.value > .5f ? 1 : -1;
            _shakeVector.y = Random.value > .5f ? 1 : -1;
            _shakeVector.z = Random.value > .5f ? 1 : -1;
        }

        public void SetCameraData(CameraData cameraData)
        {
            _cameraData = cameraData;
        }

        public void Update()
        {
            _currentTime += Time.deltaTime;
 
            if (_currentTime >= _durationSeconds)
            {
                _cameraComponent.RemoveCameraEffect(this);
                return;
            }

            ShakeCamera();
        }

        private void ShakeCamera()
        {
            var t = _currentTime / _durationSeconds;
            var magnitude = _magnitude * _animationCurve.Evaluate(1 - t);
            
            _shakeVector.x = magnitude * (_shakeVector.x > 0 ? -1 : 1);
            _shakeVector.y = magnitude * (_shakeVector.y > 0 ? -1 : 1);
            _shakeVector.z = magnitude * (_shakeVector.z > 0 ? -1 : 1);

            _cameraData.Position += _shakeVector;
        }
    }
}