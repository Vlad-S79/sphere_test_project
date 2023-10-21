using UnityEngine;

namespace Game.Level.Camera
{
    [CreateAssetMenu(fileName = "game_camera_scriptable_object", menuName = "Game/Camera", order = 1)]
    public class GameCameraScriptableObject : ScriptableObject
    {
        public Vector3 positionCamera;
        public Vector3 rotationCamera;
        public float fovCamera;
    }
}