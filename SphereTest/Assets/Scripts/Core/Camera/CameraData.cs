using UnityEngine;

namespace Core.Camera
{
    public class CameraData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        
        public float FieldOfView = 60;

        public CameraData()
        {
            Position = new Vector3();
            Rotation = new Quaternion();
        }
        
        public CameraData(float fieldOfView) : this()
        {
            FieldOfView = fieldOfView;
        }

        public CameraData(Vector3 position)
        {
            Position = position;
            Rotation = new Quaternion(); 
        }

        public CameraData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public void CopyFrom(CameraData cameraData)
        {
            Position = cameraData.Position;
            Rotation = cameraData.Rotation;
            FieldOfView = cameraData.FieldOfView;
        }
    }
}