using System;
using UnityEngine;
using Zenject;

namespace Game.Input
{
    public class InputComponent : ITickable
    {
        public event Action OnStartClick;
        public event Action OnEndClick;
        public event Action OnClick;

        private bool _isTouch;
        
        [Inject]
        private void Init(TickableManager tickableManager)
        {
            tickableManager.Add(this);
        }

        public void Tick()
        {
            #if UNITY_EDITOR
                EditorTick();
            #else
                MobileTick();
            #endif
        }

        private void MobileTick()
        {
            if (UnityEngine.Input.touchCount <= 0)
            {
                if (_isTouch)
                {
                    _isTouch = false;
                    OnEndClick?.Invoke();
                }
                
                return;
            }

            if (_isTouch)
            {
                OnClick?.Invoke();
                return;
            }
            
            var touch = UnityEngine.Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _isTouch = true;
                OnStartClick?.Invoke();
            }
            
        }

        private void EditorTick()
        {
            if(!_isTouch && UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                _isTouch = true;
                OnStartClick?.Invoke();
                return;
            }
            
            if(_isTouch && UnityEngine.Input.GetKeyUp(KeyCode.Space))
            {
                _isTouch = false;
                OnEndClick?.Invoke();
                return;
            }

            if (_isTouch)
            {
                OnClick?.Invoke();
            }
        }
    }
}