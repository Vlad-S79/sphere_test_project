using System;
using System.Collections.Generic;

namespace Core.Common
{
    //todo add preload
    public class ObjectPool<T>
    {
        private Queue<T> _container;
        private Action<T> _onInitAction;
        private Func<T> _getInstanceFunc;

        public ObjectPool(Func<T> getInstanceFunc, Action<T> onInitAction = null)
        {
            _container = new Queue<T>();
            _onInitAction = onInitAction;
            _getInstanceFunc = getInstanceFunc;
        }

        public T GetObject()
        {
            if(!_container.TryDequeue(out var tObject))
            {
                tObject = _getInstanceFunc();
                _onInitAction?.Invoke(tObject);
            }

            return tObject;
        }

        public void ReturnObject(T tObject)
        {
            _container.Enqueue(tObject);
        }

        public void Clear(Action<T> onClearAction = null)
        {
            if (onClearAction == null)
            {
                _container.Clear();
                return;    
            }
            
            while(_container.TryDequeue(out var tObject))
            {
                onClearAction.Invoke(tObject);
            }
            
            _container.Clear();
        }

        public int Count => _container.Count;
    }
}