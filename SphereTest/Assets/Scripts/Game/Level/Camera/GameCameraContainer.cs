using Core.Camera;
using UnityEngine;

namespace Game.Level.Camera
{
    public class GameCameraContainer : ICameraContainer
    {
        private CameraData _cameraData;

        private GameCameraScriptableObject _gameCameraScriptableObject;

        public GameCameraContainer(GameCameraScriptableObject gameCameraScriptableObject)
        {
            _gameCameraScriptableObject = gameCameraScriptableObject;
        }
        
        public void SetCameraData(CameraData cameraData)
        {
            _cameraData = cameraData;
            SetInitCameraData();
        }

        public void Update() { }

        private void SetInitCameraData()
        {
            _cameraData.Position = _gameCameraScriptableObject.positionCamera;
            _cameraData.Rotation = Quaternion.Euler(_gameCameraScriptableObject.rotationCamera);
            _cameraData.FieldOfView = _gameCameraScriptableObject.fovCamera;
        }
    }
}