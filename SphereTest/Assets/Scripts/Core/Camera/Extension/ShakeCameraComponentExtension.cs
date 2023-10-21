using Core.Camera.Common;

namespace Core.Camera
{
    partial class CameraComponent
    {
        private ShakeData[] _shakeData; 
        private struct ShakeData
        {
            public float DurationSeconds;
            public float Magnitude;
        }
        
        private ShakeCameraContainer _shakeCameraContainer;
        
        private void InitShakeData()
        {
            _shakeCameraContainer = new ShakeCameraContainer(this);
            
            _shakeData = new ShakeData[3];

            _shakeData[0].Magnitude = 0.08f;
            _shakeData[0].DurationSeconds = 0.1f;
        
            _shakeData[1].Magnitude = .1f;
            _shakeData[1].DurationSeconds = 0.1f;
        
            _shakeData[2].Magnitude = .14f;
            _shakeData[2].DurationSeconds = 0.1f;
        }
        
        public void Shake(ShakeType shakeType)
        {
            var shakeDataIndex = (int) shakeType;
            _shakeCameraContainer.SetData(_shakeData[shakeDataIndex].DurationSeconds, 
                _shakeData[shakeDataIndex].Magnitude);
            
            AddCameraEffect(_shakeCameraContainer);
        }
    }
    
    public enum ShakeType
    {
        Light,
        Medium,
        Heavy
    }
}