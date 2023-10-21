using System;
using System.Collections;
using Core.Common;
using UnityEngine;

namespace Core.Ui
{
    public class WindowAnimator
    {
        private readonly Animator _animator;
        private MonoBehaviour _behaviour;
        
        private int _layerIndex;

        public WindowAnimator(MonoBehaviour behaviour, int layerIndex = 0)
        {
            _behaviour = behaviour;
            _animator = _behaviour.GetComponent<Animator>();

            _layerIndex = layerIndex;
        }

        public void PlayAnimation(int animationId, Action onComplete = null)
        {
            SetEnableAnimator(true);
            _animator.Play(animationId);
            
            var duration = _animator.GetCurrentAnimatorStateInfo(_layerIndex).length;

            _behaviour.StartCoroutine(
                DoActionAfterTimeEnumerator(duration, () =>
                {
                    SetEnableAnimator(false);
                    onComplete?.Invoke();
                }));
        }
        
        private IEnumerator DoActionAfterTimeEnumerator(float timeSeconds, Action action)
        {
            yield return new WaitForSeconds(timeSeconds);
            action();
        }
        
        public void SetAnimation(int animationId, float normalizedTime)
        {
            _animator.Play(animationId, _layerIndex, normalizedTime);
        }

        public void SetEnableAnimator(bool value)
        {
            _animator.enabled = value;
        }
    }

    public static class WindowAnimations
    {
        public static readonly int CloseAnimationID = Animator.StringToHash("close");
        public static readonly int OpenAnimationID = Animator.StringToHash("open");
    }
}