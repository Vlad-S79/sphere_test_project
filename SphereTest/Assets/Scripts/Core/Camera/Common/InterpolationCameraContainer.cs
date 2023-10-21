// #define INTERPOLATION_DEBUG

using System;
using System.Collections.Generic;
using Core.Common;
using UnityEngine;

namespace Core.Camera.Common
{
    public class InterpolationCameraContainer : ICameraContainer
    {
        private CameraData _cameraData;
        private ICameraContainer _cameraContainer;
        
        private readonly CameraComponent _cameraComponent;

        private readonly ObjectPool<CameraDataInterpolation> _interpolationCameraDataPool;
        private List<CameraDataInterpolation> _cameraDataInterpolationList;

        private CameraDataInterpolation _oldCameraDataInterpolation;

        private Action _onCompleteAction;

        public InterpolationCameraContainer(CameraComponent cameraComponent, 
            AnimationCurve animationCurve)
        {
            _cameraComponent = cameraComponent;
            
            _cameraDataInterpolationList = new List<CameraDataInterpolation>();
            _interpolationCameraDataPool = new ObjectPool<CameraDataInterpolation>(
                () => new CameraDataInterpolation(animationCurve));
        }

        public void SetData(ICameraContainer cameraContainer, float duration)
        {
            if (_cameraComponent.GetCameraContainer().GetType() != typeof(InterpolationCameraContainer))
            {
                _oldCameraDataInterpolation = null;
                _cameraContainer = _cameraComponent.GetCameraContainer();
                Clear();
            }

            // ReturnBackOldCameraDataInterpolation();
            _oldCameraDataInterpolation?.SetActionOnComplete(null);
            
            var cameraDataInterpolation = _interpolationCameraDataPool.GetObject();
            cameraDataInterpolation.SetCameraData(_cameraData);
            cameraDataInterpolation.SetData(cameraContainer, duration);
            cameraDataInterpolation.SetActionOnComplete(() => OnCompleteInterpolation(cameraContainer));

            _oldCameraDataInterpolation = cameraDataInterpolation;
            _cameraDataInterpolationList.Add(cameraDataInterpolation);
        }

        private void OnCompleteInterpolation(ICameraContainer cameraContainer)
        {
            _cameraComponent.SetCameraContainer(cameraContainer);
            
            _onCompleteAction?.Invoke();
            _onCompleteAction = null;
        }

        // No Need In Real Game 
        // private void ReturnBackOldCameraDataInterpolation()
        // {
        //     Action clearListAfterComplete = null;
        //     
        //     foreach (var cameraDataInterpolation in _cameraDataInterpolationList)
        //     {
        //         if (!cameraDataInterpolation.IsActive())
        //         {
        //             cameraDataInterpolation.SetActionOnComplete(null);
        //             _interpolationCameraDataPool.ReturnObject(cameraDataInterpolation);
        //             
        //             clearListAfterComplete += () =>
        //             {
        //                 _cameraDataInterpolationList.Remove(cameraDataInterpolation);
        //             };
        //         }
        //     }
        //     
        //     clearListAfterComplete?.Invoke();
        // }

        private void Clear()
        {
            foreach (var cameraDataInterpolation in _cameraDataInterpolationList)
            {
                cameraDataInterpolation.SetActionOnComplete(null);
                _interpolationCameraDataPool.ReturnObject(cameraDataInterpolation);
            }
            
            _cameraDataInterpolationList.Clear();
        }

        public void SetOnCompleteAction(Action onCompleteAction)
        {
            _onCompleteAction += onCompleteAction;
        }

        public void SetCameraData(CameraData cameraData)
        {
            _cameraData = cameraData;
            // _cameraContainer.SetCameraData(_cameraData);
            _oldCameraDataInterpolation.SetCameraData(_cameraData);
        }

        public void Update()
        {
            #if INTERPOLATION_DEBUG
                DebugLog("in", _cameraContainer.GetType().ToString());
            #endif
            
            _cameraContainer.Update();
            
            #if INTERPOLATION_DEBUG
                DebugLog("out", _cameraContainer.GetType().ToString());
            #endif
            
            foreach (var cameraDataInterpolation in _cameraDataInterpolationList)
            {
                #if INTERPOLATION_DEBUG
                    DebugLog("in", cameraDataInterpolation.GetCameraContainer().GetType().ToString());
                #endif
                
                cameraDataInterpolation.Update();
                
                #if INTERPOLATION_DEBUG
                    DebugLog("out", cameraDataInterpolation.GetCameraContainer().GetType().ToString());
                #endif
            }
            
            #if INTERPOLATION_DEBUG
                Debug.LogError("EndFrame");
            #endif
        }
        
        #if INTERPOLATION_DEBUG
            private void DebugLog(string message, string type)
            {
                Debug.LogError(message + " : " + CameraDataToString() + " : " + type) ;
            }

            private string CameraDataToString()
            {
                return _cameraData.FieldOfView.ToString();
            }
        #endif
    }
}