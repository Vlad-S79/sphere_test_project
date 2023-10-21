using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "scriptable_object_bootstrap", menuName = "Game/Scriptable Object Bootstrap", order = 0)]
public class ScriptableObjectBootstrap : ScriptableObjectInstaller
{
    [SerializeField] private List<ScriptableObject> _container;
    
    public override void InstallBindings()
    {
        foreach (var scriptableObject in _container)
        {
            if (scriptableObject is IScriptableObjectInstaller iScriptableObjectInstaller)
            {
                iScriptableObjectInstaller.Init();
            }

            Container.Bind(scriptableObject.GetType()).FromInstance(scriptableObject).AsSingle();
        }
    }
}

public interface IScriptableObjectInstaller
{
    public void Init();
}