using System.Collections;
using Core.Common;
using Core.Haptic;
using Core.Ui;
using Game.Input;
using Game.Level;
using Game.UI;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    public class PlayerComponent
    {
        private Transform _player;
        private ObjectPool<Transform> _bulletPool;
        
        private PlayerScriptableObject _playerScriptableObject;

        private InputComponent _inputComponent;
        private MonoBehaviour _behaviour;

        private Transform _playerPath;

        private LevelComponent _levelComponent;

        private AnimationCurve _animationCurve;

        private ParticleSystem _playerParticle;

        private UiComponent _uiComponent;
        private HapticComponent _hapticComponent;

        [Inject]
        public void Init(PlayerScriptableObject playerScriptableObject, 
            InputComponent inputComponent, MonoBehaviour monoBehaviour,
            LevelComponent levelComponent, LevelScriptableObject levelScriptableObject,
            UiComponent uiComponent, HapticComponent hapticComponent)
        {
            _uiComponent = uiComponent;
            _hapticComponent = hapticComponent;
            
            _behaviour = monoBehaviour;
            _levelComponent = levelComponent;
            _playerScriptableObject = playerScriptableObject;

            _player = Object.Instantiate(_playerScriptableObject.player).transform;
            _player.position += Vector3.right * _playerScriptableObject.playerStartOffset;
            SetPlayerScale(_playerScriptableObject.maxPlayerScale);

            _playerPath = Object.Instantiate(_playerScriptableObject.playerPath).transform;

            _bulletPool = new ObjectPool<Transform>(GetBulletInstance);

            _inputComponent = inputComponent;
            
            _inputComponent.OnStartClick += StartCreateBullet;
            _inputComponent.OnEndClick += EndCreateBullet;
            _inputComponent.OnClick += BulletChangeScale;
            
            ResetPlayerPath();
            UpdatePlayerPathScale();
            
            _animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            _animationCurve.postWrapMode = WrapMode.PingPong;

            _playerParticle = Object.Instantiate(levelScriptableObject.treeParticle);
            _playerParticle.gameObject.SetActive(false);
        }

        private Transform GetBulletInstance()
        {
            return Object.Instantiate(_playerScriptableObject.bullet).transform;
        }

        private void SetPlayerScale(float scale)
        {
            _player.localScale = Vector3.one * scale;
            
            var playerPosition = _player.position;
            playerPosition.y = scale / 2;
            _player.position = playerPosition;
        }

        private Transform _currentBullet;
        private float _currentBulletScale;

        private void StartCreateBullet()
        {
            if(_isLevelComplete) return;
            
            if(_currentBullet != null) return;
            _currentBullet = _bulletPool.GetObject();
            _currentBullet.gameObject.SetActive(true);
            
            _currentBulletScale = _playerScriptableObject.minBulletScale;
            _oldPlayerScale = _player.localScale.x;
            
            SetCurrentBulletScale(_currentBulletScale);

        }

        private void EndCreateBullet()
        {
            if(_currentBullet == null) return;

            var result = _levelComponent.CalcBullet(_currentBullet);
            _behaviour.StartCoroutine(BulletAnim(result, _currentBullet));
            
            _currentBullet = null;
        }

        private IEnumerator BulletAnim(float endPosition, Transform bullet)
        {
            while (bullet.position.x < endPosition)
            {
                bullet.position += Vector3.right * (_playerScriptableObject.bulletSpeed * Time.deltaTime);
                yield return 0;
            }
            
            _levelComponent.CalcBulletExplosion(bullet, CheckLevelComplete);
            bullet.gameObject.SetActive(false);
            _bulletPool.ReturnObject(bullet);
        }

        private float _oldPlayerScale;
        private void BulletChangeScale()
        {
            if(_isLevelComplete || _currentBullet == null) return;
            
            _currentBulletScale *= 1 + Time.deltaTime * _playerScriptableObject.bulletScaleSpeed;

            _player.localScale = Vector3.one * (_oldPlayerScale - _currentBulletScale * _playerScriptableObject.playerScaleSpeed);
            
            var playerPos = _player.position;
            playerPos.y = _player.localScale.y / 2;
            _player.position = playerPos;
            
            UpdatePlayerPathScale();
            
            SetCurrentBulletScale(_currentBulletScale);

            if (_player.localScale.x < _playerScriptableObject.minPlayerScale)
            {
                EndCreateBullet();
                _isLevelComplete = true;
                _player.gameObject.SetActive(false);
                
                _playerParticle.transform.position = _player.position;
                _playerParticle.gameObject.SetActive(true);
                
                OnFail();
            }

            if (_currentBullet.localScale.x >= _playerScriptableObject.maxPlayerScale)
            {
                EndCreateBullet();
            }
        }

        private void OnFail()
        {
            _hapticComponent.Vibrate(Haptic.Medium);
            _uiComponent.GetWindow<RestartWindow>().Open();
        }

        private void SetCurrentBulletScale(float scale)
        {
            var halfPlayerScale = _player.localScale / 2;
            var halfScale = scale / 2;
            _currentBullet.localScale = Vector3.one * scale;
            _currentBullet.position = new Vector3(
                _player.position.x + halfPlayerScale.x  + halfScale,
                Mathf.Max(halfPlayerScale.y, halfScale),
                0);
        }

        private void ResetPlayerPath()
        {
            var levelSize = _levelComponent.GetLevelSize();
            var playerPathScale = levelSize - _playerScriptableObject.pathOffset;
            var position = (playerPathScale) / 2 + _playerScriptableObject.pathOffset;

            _playerPath.localScale = new Vector3(playerPathScale, 1, 1);
            _playerPath.position = new Vector3(position, 0.001f, 0);
        }

        private void UpdatePlayerPathScale()
        {
            var oldScale = _playerPath.localScale;
            oldScale.y = _player.localScale.x;
            
            _playerPath.localScale = oldScale;
        }

        private bool _isLevelComplete;
        
        public void CheckLevelComplete()
        {
            if(_isLevelComplete) return;
            _isLevelComplete = _levelComponent.CheckLevelComplete(GelPlayerScale());
            
            if (_isLevelComplete)
            {
                OnLevelComplete();
            }
        }

        private void OnLevelComplete()
        {
            var door = _levelComponent.GetDoor();
            door.SetAnim(true);

            _behaviour.StartCoroutine(FinalJumpPlayerAnim());
        }

        private IEnumerator FinalJumpPlayerAnim()
        {
            var levelSize = _levelComponent.GetLevelSize();

            float t;
            while (_player.position.x < levelSize + _player.localScale.x / 2)
            {
                _hapticComponent.Vibrate(Haptic.Light);
                
                t = 0;
                var oldPlayerPos = _player.position;
                while (t < _playerScriptableObject.finalJumpTimeSeconds)
                {
                    t += Time.deltaTime;
                    var time = t / _playerScriptableObject.finalJumpTimeSeconds;
                    var animT = _animationCurve.Evaluate(time);
                    var doubleAnimT = _animationCurve.Evaluate(time * 2);

                    var pos = new Vector3(
                        _playerScriptableObject.finalJumpSize.x * animT,
                        _playerScriptableObject.finalJumpSize.y * doubleAnimT,
                        0);

                    _player.position = oldPlayerPos + pos;
                    
                    if(_player.position.x > levelSize + _player.localScale.x / 2) break;
                    yield return 0;
                }
            }
            
            _hapticComponent.Vibrate(Haptic.Light);
            _uiComponent.GetWindow<FinalWindow>().Open();
        }

        public void ResetPlayer()
        {
            _isLevelComplete = false;
            _player.gameObject.SetActive(true);
            SetPlayerScale(_playerScriptableObject.maxPlayerScale);
            UpdatePlayerPathScale();
        }

        private float GelPlayerScale() => _player.localScale.x;

    }
}