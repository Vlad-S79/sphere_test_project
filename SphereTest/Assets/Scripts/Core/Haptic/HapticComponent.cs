using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
    using System.Runtime.InteropServices;
#endif

namespace Core.Haptic
{
    public class HapticComponent
    {
        private const string KeyIsEnableHaptic = "is_enable_haptic";

        private bool _isEnableHaptic;
        public bool IsEnableHaptic
        {
            get => _isEnableHaptic;
            set
            {
                if(_isEnableHaptic == value) return;
                
                _isEnableHaptic = value;
                PlayerPrefs.SetInt(KeyIsEnableHaptic, _isEnableHaptic ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        
        #if UNITY_ANDROID && !UNITY_EDITOR
            private AndroidJavaObject _androidHaptic;
        #endif
        
        public HapticComponent()
        {
            _isEnableHaptic = PlayerPrefs.GetInt(KeyIsEnableHaptic, 1) == 1;
            
            #if UNITY_ANDROID && !UNITY_EDITOR
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                _androidHaptic = new AndroidJavaObject("com.colours.AndroidHaptic");
                _androidHaptic.Call("setContext", currentActivity);        
            #endif
        }
        
        #if UNITY_EDITOR
            public void Vibrate(Haptic haptic)
            {
                if(!_isEnableHaptic) return;
                Debug.Log("Vibrate : " + haptic);
            }
        
        #elif UNITY_IOS
            [DllImport("__Internal")]
            private static extern void TriggerHapticFeedback(int style);

            public void Vibrate(Haptic haptic)
            {
                if(!_isEnableHaptic) return;
                
                TriggerHapticFeedback((int) haptic);
            }
        #elif UNITY_ANDROID
            public void Vibrate(Haptic haptic)
            {
                _androidHaptic.Call("vibrate", (int)haptic);
            }
        #endif
    }
}