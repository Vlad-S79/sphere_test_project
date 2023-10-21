using System;
using System.Collections.Generic;
using Core.Common;
using Core.Ui.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace Core.Ui
{
    // todo move window type to attribute
    
    public class UiComponent
    {
        private Dictionary<Type, Window> _dictionary;
        private Dictionary<Type, ObjectPool<Window>> _poolDictionary;

        private int[] _siblingIndexes;
        
        private EventSystem _eventSystem;
        private Canvas _canvas;

        private UiComponentScriptableObject _uiComponentScriptableObject;
        private DiContainer _container;

        public EventSystem GetEventSystem => _eventSystem;
        
        [Inject]
        private void Init(UiComponentScriptableObject uiComponentScriptableObject, DiContainer container)
        {
            _container = container;
            
            _dictionary = new Dictionary<Type, Window>();
            _poolDictionary = new Dictionary<Type, ObjectPool<Window>>();
            
            _uiComponentScriptableObject = uiComponentScriptableObject;
            _siblingIndexes = new int [Enum.GetNames(typeof(WindowType)).Length];
            
            LoadResourcesToScene();
            SceneManager.activeSceneChanged += OnChangeScene;
        }

        private void LoadResourcesToScene()
        {
            _canvas = Object.Instantiate(_uiComponentScriptableObject.Canvas);
            _eventSystem = Object.Instantiate(_uiComponentScriptableObject.EventSystem);

            _canvas.name = "canvas";
            _eventSystem.name = "event_system";
        }
        
        private void OnChangeScene(Scene first, Scene second)
        {
            if(first.name == null) return;
            
            ResetSceneData();
            LoadResourcesToScene();
        }

        public T GetWindow<T>() where T : Window
        {
            var type = typeof(T);

            if (_dictionary.ContainsKey(type))
            {
                return (T)_dictionary[type];
            }

            var window = InstanceWindow<T>();
            _dictionary.Add(type, window);

            return window;
        }

        public T GetPoolWindow<T>() where T : Window
        {
            var type = typeof(T);

            if (_poolDictionary.TryGetValue(type, out var objectPool))
            {
                return (T) objectPool.GetObject();
            }

            var poolObject = new ObjectPool<Window>(InstanceWindow<T>);
            
            _poolDictionary.Add(type, poolObject);
            return (T) poolObject.GetObject();
        }

        public void ReturnPoolWindow<T>(T window) where T : Window
        {
            var type = typeof(T);

            if (_poolDictionary.TryGetValue(type, out var objectPool))
            {
                objectPool.ReturnObject(window);
            }
        }

        private T InstanceWindow<T>() where T : Window
        {
            var type = typeof(T);

            if (!_uiComponentScriptableObject.Container.TryGetValue(type, out var reference))
            {
                Debug.LogError(" *** Window Reference Container NOT Contains " + type.ToString());
                return null;
            }

            reference.gameObject.SetActive(false);
            var window = _container.InstantiatePrefabForComponent<T>(reference, _canvas.transform);
            reference.gameObject.SetActive(true);
 
            _siblingIndexes[(int) window.GetWindowType()]++;

            return window;
        }

        public int GetSiblingIndex(WindowType type)
        {
            var index = (int) type;
            var result = 0;
            
            for (int i = 0; i < index; i++)
            {
                result += _siblingIndexes[i];
            }
            
            return result + 1;
        }

        private void ResetSceneData()
        {
            for (var i = 0; i < _siblingIndexes.Length; i++)
            {
                _siblingIndexes[i] = 0;
            }
            
            _dictionary.Clear();
            _poolDictionary.Clear();
        }
    }
}