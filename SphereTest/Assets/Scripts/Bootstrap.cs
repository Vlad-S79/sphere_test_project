using Zenject;
using Core.Camera;
using Core.Haptic;
using Core.Ui;
using Game;
using Game.Input;
using Game.Level;
using Game.Level.Camera;
using Game.Player;
using UnityEngine;

public class Bootstrap : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<MonoBehaviour>().FromInstance(this).AsSingle();

        InstallBindingsCore();
        InstallBindingsGame();
        
        var gameLogicComponent = new GameEnterPoint();
        Container.Inject(gameLogicComponent);
    }
    
    private void Bind<T>() where T : new()
    {
        var myComponent = new T();
        Container.Inject(myComponent);
        
        Container.Bind<T>().FromInstance(myComponent).AsSingle();
    }

    private void InstallBindingsCore()
    {
        Bind<CameraComponent>();
        Bind<UiComponent>();
        Bind<HapticComponent>();
    }

    private void InstallBindingsGame()
    {
        Bind<InputComponent>();
        Bind<GameCameraComponent>();
        Bind<LevelComponent>();
        Bind<PlayerComponent>();
    }
}