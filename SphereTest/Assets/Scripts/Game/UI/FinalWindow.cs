using Core.Ui;
using Game.Level;
using Game.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Game.UI
{
    public class FinalWindow : Window
    {
        [SerializeField] private Button _button;

        [Inject]
        private void Init() { }
        
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}