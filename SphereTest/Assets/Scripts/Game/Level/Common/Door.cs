using System.Collections;
using UnityEngine;

namespace Game.Level.Common
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Transform door;
        private float _openCloseDoorDurationSeconds = .2f;
        
        private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public bool IsOpen { get; private set; } = true;

        public void Set(bool isOpen)
        {
            if(IsOpen == isOpen) return;
            IsOpen = isOpen;

            var value = IsOpen ? 1 : 0;
            door.localScale = new Vector3(value, 1, 1);
        }

        public void SetAnim(bool isOpen)
        {
            if(IsOpen == isOpen) return;
            IsOpen = isOpen;

            StartCoroutine(SetAnimEnumerator());
        }

        private IEnumerator SetAnimEnumerator()
        {
            var t = 0f;
            while (t < _openCloseDoorDurationSeconds)
            {
                t += Time.deltaTime;
                var time = _animationCurve.Evaluate(t / _openCloseDoorDurationSeconds);

                if (!IsOpen)
                {
                    time = 1 - time;
                }

                door.localScale = new Vector3(time, 1, 1);
                yield return 0;
            }

            Set(IsOpen);
        }
    }
}