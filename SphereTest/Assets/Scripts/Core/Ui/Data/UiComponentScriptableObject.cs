using System;
using System.Collections.Generic;
using Core.Common.SerializedCollections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Ui.Data
{
    [CreateAssetMenu(fileName = "ui_component_scriptable_object", menuName = "Core/UIComponent", order = 1)]
    public class UiComponentScriptableObject : ScriptableObject, IScriptableObjectInstaller
    {
        [SerializeField] private SerializedHashSet<Window> _container;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private EventSystem _eventSystem;
        
        public Dictionary<Type, Window> Container { get; private set; }
        public Canvas Canvas => _canvas;
        public EventSystem EventSystem => _eventSystem;
        
        public void Init()
        {
            Container = _container.ToDictionary();
        }
    }
}