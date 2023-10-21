// #define CAMERA_DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Camera
{
    public class CameraBehaviour : ILateTickable
    {
        private ICameraContainer _cameraContainer;
        private HashSet<ICameraContainer> _cameraContainerEffect;
        
        private UnityEngine.Camera _camera;
        private Transform _cameraTransform;

        private CameraData _cameraData;

        private Action _onCompleteEffects;

        public CameraBehaviour(UnityEngine.Camera camera, Transform cameraTransform)
        {
            _camera = camera;
            _cameraTransform = cameraTransform;

            _cameraData = new CameraData();
            _cameraContainerEffect = new HashSet<ICameraContainer>();
        }

        public void SetCameraContainer(ICameraContainer cameraContainer)
        {
            _cameraContainer = cameraContainer;
            cameraContainer.SetCameraData(_cameraData);
        }

        public void AddCameraEffect(ICameraContainer cameraContainerEffect)
        {
            if (!_cameraContainerEffect.Contains(cameraContainerEffect))
            {
                cameraContainerEffect.SetCameraData(_cameraData);
                _cameraContainerEffect.Add(cameraContainerEffect);
            }
        }

        public void RemoveCameraEffect(ICameraContainer cameraContainerEffect)
        {
            if(_cameraContainerEffect.Contains(cameraContainerEffect))
                _onCompleteEffects += () 
                    => _cameraContainerEffect.Remove(cameraContainerEffect);
        }

        // public void LateTick()
        // {
        //     UpdateCamera();
        // }

        public void UpdateCameraData()
        {
            _camera.fieldOfView = _cameraData.FieldOfView;
            _cameraTransform.position = _cameraData.Position;
            _cameraTransform.rotation = _cameraData.Rotation;
        }

        public void LateTick()
        {
            if(_cameraContainer == null) return;
            
            #if CAMERA_DEBUG
                Debug.LogError("in: " + CameraDataToString() + " : " + _cameraContainer.GetType().ToString());
            #endif
            
            _cameraContainer.Update();
            
            #if CAMERA_DEBUG
                Debug.LogError("out: " + CameraDataToString() + " : " + _cameraContainer.GetType().ToString());
            #endif
            
            
            foreach (var cameraContainerEffect in _cameraContainerEffect)
            {
                #if CAMERA_DEBUG
                    Debug.LogError("in: " + CameraDataToString() + " : " + cameraContainerEffect.GetType().ToString());
                #endif
                
                cameraContainerEffect.Update();
                
                #if CAMERA_DEBUG
                    Debug.LogError("out: " + CameraDataToString() + " : " + cameraContainerEffect.GetType().ToString());
                #endif
            }
            
            #if CAMERA_DEBUG
                Debug.LogError("EndFrame");
            #endif

            _onCompleteEffects?.Invoke();
            _onCompleteEffects = null;
            
            UpdateCameraData();
        }
        
        #if CAMERA_DEBUG
            private string CameraDataToString()
            {
                return _cameraData.FieldOfView.ToString();
            }
        #endif
        

        public CameraData GetCameraData() => _cameraData;
    }
}