using System;
using UnityEngine;

namespace Core.Camera.Common
{
    public class CameraDataInterpolation
    {
        private readonly AnimationCurve _animationCurve;
        private ICameraContainer _cameraContainer;

        private float _duration;
        private float _currentTime;

        private CameraData _cameraData;
        private readonly CameraData _toCameraData;

        private Action _onCompleteAction;

        private bool _isActive;

        public CameraDataInterpolation(AnimationCurve animationCurve)
        {
            _animationCurve = animationCurve;
            _toCameraData = new CameraData();
        }

        public void SetCameraData(CameraData cameraData)
        {
            _cameraData = cameraData;
        }

        public ICameraContainer GetCameraContainer() => _cameraContainer;
        
        public void SetData(ICameraContainer cameraContainer, float duration)
        {
            _duration = duration;

            _cameraContainer = cameraContainer;
            _cameraContainer.SetCameraData(_toCameraData);
            
            _currentTime = 0;
            _isActive = true;
        }

        public void SetActionOnComplete(Action onCompleteAction)
        {
            _onCompleteAction = onCompleteAction;
        }
        
        public void Update()
        {
            if (!_isActive)
            {
                _cameraData.Position = _toCameraData.Position;
                _cameraData.Rotation = _toCameraData.Rotation;
                _cameraData.FieldOfView = _toCameraData.FieldOfView;
                return;
            }
            
            if (!_isActive) return;
            
            _currentTime += Time.deltaTime;
            // Debug.LogError(_currentTime);

            _cameraContainer.Update();
            Interpolate();
            
            if (_currentTime >= _duration)
            {
                _onCompleteAction?.Invoke();
                _onCompleteAction = null;
                _isActive = false;
            }
        }
        
        private void Interpolate()
        {
            var interpolationValue = _animationCurve.Evaluate(_currentTime / _duration);
            
            _cameraData.FieldOfView = Mathf.Lerp(_cameraData.FieldOfView,_toCameraData.FieldOfView, interpolationValue);
            _cameraData.Position = Vector3.Lerp(_cameraData.Position,_toCameraData.Position, interpolationValue);
            _cameraData.Rotation = Quaternion.Lerp(_cameraData.Rotation,_toCameraData.Rotation, interpolationValue);
        }

        public bool IsActive() => _isActive;
    }
}