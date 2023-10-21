using Core.Ui;
using Game.Level;
using Game.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI
{
    public class RestartWindow : Window
    {
        [SerializeField] private Button _button;

        private PlayerComponent _playerComponent;
        private LevelComponent _levelComponent;

        [Inject]
        private void Init(LevelComponent levelComponent, PlayerComponent playerComponent)
        {
            _levelComponent = levelComponent;
            _playerComponent = playerComponent;
        }
        
        protected override void OnOpen()
        {
            _button.onClick.AddListener(RestartLevel);
        }

        protected override void OnClose()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void RestartLevel()
        {
            _levelComponent.ResetLevel();
            _playerComponent.ResetPlayer();
            Close();
        }
    }
}