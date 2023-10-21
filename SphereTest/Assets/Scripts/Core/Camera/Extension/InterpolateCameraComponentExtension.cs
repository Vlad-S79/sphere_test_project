using System;
using Core.Camera.Common;
using UnityEngine;

namespace Core.Camera
{
    partial class CameraComponent
    {
        private InterpolationCameraContainer _interpolationCameraContainer;
        private AnimationCurve _interpolationAnimationCurve;
        
        private void InitInterpolationData()
        {
            _interpolationAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            _interpolationCameraContainer = new InterpolationCameraContainer(this, _interpolationAnimationCurve);
        }
        
        public void Interpolate(ICameraContainer cameraContainer, float duration)
        {
            _interpolationCameraContainer.SetData(cameraContainer, duration);
            SetCameraContainer(_interpolationCameraContainer);
        }
        
        public void Interpolate(ICameraContainer cameraContainer, float duration, Action onComplete)
        {
            _interpolationCameraContainer.SetOnCompleteAction(onComplete);
            Interpolate(cameraContainer, duration);
        }
    }
}