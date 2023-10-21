using Core.Camera;
using UnityEngine;
using Zenject;

namespace Game.Level.Camera
{
    public class GameCameraComponent
    {
        private GameCameraContainer _gameCameraContainer;
        
        private GameCameraScriptableObject _gameCameraScriptableObject;
        private CameraComponent _cameraComponent;
        
        [Inject]
        private void Init(GameCameraScriptableObject gameCameraScriptableObject,
            CameraComponent cameraComponent)
        {
            _gameCameraScriptableObject = gameCameraScriptableObject;
            _cameraComponent = cameraComponent;

            _gameCameraContainer = new GameCameraContainer(_gameCameraScriptableObject);
            _cameraComponent.SetCameraContainer(_gameCameraContainer);

            var camera = _cameraComponent.GetCamera();
            camera.farClipPlane = 100;
        }

        private Vector2 CalcMinMaxXRenderCameraPosition()
        {
            //todo
            return Vector2.negativeInfinity;
        }
    }
}