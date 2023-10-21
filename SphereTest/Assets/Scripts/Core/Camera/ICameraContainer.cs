namespace Core.Camera
{
    public interface ICameraContainer
    {
        public void SetCameraData(CameraData cameraData);
        public void Update();
    }
}