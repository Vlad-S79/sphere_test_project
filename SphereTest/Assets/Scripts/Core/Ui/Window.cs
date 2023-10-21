using UnityEngine;
using Zenject;

namespace Core.Ui
{
    public abstract class Window : MonoBehaviour
    {
        protected WindowAnimator windowAnimator;
        protected UiComponent uiComponent;

        [SerializeField] private WindowType _windowType;
        [SerializeField] private bool _isHaveAnimation;

        [Inject]
        private void Init(UiComponent uiComponent)
        {
            this.uiComponent = uiComponent;
            
            if (_isHaveAnimation)
            {
                var animator = GetComponent<Animator>();
                windowAnimator = new WindowAnimator(this);
                animator.enabled = false;
            }
        }
        
        public void Open()
        {
            var siblingIndex = uiComponent.GetSiblingIndex(_windowType);
            transform.SetSiblingIndex(siblingIndex);
            
            if (!_isHaveAnimation)
            {
                gameObject.SetActive(true);
                OnOpen();
            }
            else
            {
                OpenAnim();
            }
        }

        private void OpenAnim()
        {
            gameObject.SetActive(true);
            windowAnimator.PlayAnimation(WindowAnimations.OpenAnimationID, OnOpen);
        }

        private void CloseAnim()
        {
            windowAnimator.PlayAnimation(WindowAnimations.CloseAnimationID, 
                () => gameObject.SetActive(false));
        }

        public void Close()
        {
            OnClose();

            if (!_isHaveAnimation)
            {
                gameObject.SetActive(false);
            }
            else
            {
                CloseAnim();
            }
        }
        
        protected abstract void OnOpen();
        protected abstract void OnClose();

        public WindowType GetWindowType() => _windowType;
    }
}