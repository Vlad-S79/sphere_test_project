using System;
using System.Collections;
using System.Collections.Generic;
using Core.Common;
using Core.Haptic;
using Game.Level.Common;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game.Level
{
    public class LevelComponent
    {
        private List<TreeData> _levelTree;
        private List<Transform> _levelBackgroundTree;

        private ObjectPool<Transform> _treePool;
        private LevelScriptableObject _levelScriptableObject;

        private ObjectPool<ParticleSystem> _treeParticlePool;

        private GameObject _groundGameObject;
        private Material _groundMaterial;

        private float _levelSize;
        private readonly int _mainTex = Shader.PropertyToID("_MainTex");

        private Door _door;

        private Transform _parentTreeTransform;
        private MonoBehaviour _behaviour;

        private HapticComponent _hapticComponent;

        [Inject]
        private void Init(LevelScriptableObject levelScriptableObject, MonoBehaviour behaviour,
            HapticComponent hapticComponent)
        {
            _behaviour = behaviour;
            _levelScriptableObject = levelScriptableObject;

            _hapticComponent = hapticComponent;

            _treePool = new ObjectPool<Transform>(GetInstanceTree, RandomizeScaleTree);
            _levelBackgroundTree = new List<Transform>();

            _treeParticlePool = new ObjectPool<ParticleSystem>(GetInstanceParticle);

            _groundGameObject = Object.Instantiate(_levelScriptableObject.ground);
            _groundMaterial = _groundGameObject.GetComponent<MeshRenderer>().material;

            _parentTreeTransform = new GameObject("tree").transform;
            _levelTree = new List<TreeData>();
            
            GenerateLevel();
            UpdateGround();
        }

        private void UpdateGround()
        {
            var offsetSize = _levelScriptableObject.offsetBeforeLevel + _levelScriptableObject.offsetAfterLevel;
            var transform = _groundGameObject.transform;
            transform.position =
                new Vector3(_levelSize / 2 - _levelScriptableObject.offsetBeforeLevel / _levelScriptableObject.groundTextureScaler, 0, 0);

            transform.localScale = new Vector3(
                (_levelSize + offsetSize) / _levelScriptableObject.groundTextureScaler, 
                1, 
                _levelScriptableObject.defaultLevelWith / _levelScriptableObject.groundTextureScaler);

            var groundTextureScale = new Vector2(
                (_levelSize + offsetSize) / _levelScriptableObject.groundTextureScaler, 
                _levelScriptableObject.defaultLevelWith / _levelScriptableObject.groundTextureScaler); 
            
            _groundMaterial.SetTextureScale(_mainTex, groundTextureScale * _levelScriptableObject.groundTextureScaler);
        }

        private Transform GetInstanceTree()
        {
            var tree = Object.Instantiate(_levelScriptableObject.tree);
            var treeTransform = tree.transform;
            treeTransform.parent = _parentTreeTransform;
            return treeTransform;
        }

        private ParticleSystem GetInstanceParticle()
        {
            var particle = Object.Instantiate(_levelScriptableObject.treeParticle);
            particle.gameObject.SetActive(false);
            return particle;
        }

        private void RandomizeScaleTree(Transform transform)
        {
            transform.localScale = new Vector3(
                GetRandomScaleValue(),
                GetRandomScaleValue(),
                GetRandomScaleValue());
        }

        private float GetRandomScaleValue() => 
            .5f + Random.value * .2f - .1f;

        private void GenerateLevel()
        {
            _levelSize = 0;
            
            var cellSize = _levelScriptableObject.cellTreeSize;
            var halfCellSize = cellSize / 2;

            int rawCellAmountX;
            var rawCellAmountY = Mathf.FloorToInt(_levelScriptableObject.defaultLevelWith / cellSize);

            for (var i = 0; i <= _levelScriptableObject.pointAmount; i++)
            {
                rawCellAmountX = Mathf.FloorToInt(_levelScriptableObject.sizeLevelToPoint / cellSize);

                for (var x = 0; x < rawCellAmountX; x++)
                {
                    for (var y = 0; y < rawCellAmountY; y++)
                    {
                        var position = new Vector3(
                                           _levelSize + cellSize * x, 
                                           .5f,
                                           cellSize * y - _levelScriptableObject.defaultLevelWith / 2 + cellSize) 
                                       + new Vector3(
                                           Random.Range(-halfCellSize, halfCellSize),
                                           0,
                                           Random.Range(-halfCellSize, halfCellSize));
                        
                        var scale = new Vector3(
                            GetRandomScaleValue(),
                            GetRandomScaleValue(),
                            GetRandomScaleValue());

                        
                        var treeObject = _treePool.GetObject();
                        treeObject.position = position;
                        treeObject.localScale = scale;

                        if (position.z > -_levelScriptableObject.gameLevelSizeWith &&
                            position.z < _levelScriptableObject.gameLevelSizeWith)
                        {
                            var tree = new TreeData(treeObject);
                            _levelTree.Add(tree);
                        }
                        else
                        {
                            _levelBackgroundTree.Add(treeObject);
                        }
                    }
                }
                _levelSize += _levelScriptableObject.sizeLevelToPoint;

                var isCheckPoint = i == _levelScriptableObject.pointAmount;
                var size = !isCheckPoint
                    ? _levelScriptableObject.checkPointSize
                    : _levelScriptableObject.doorSize;

                rawCellAmountX = Mathf.FloorToInt((size.x + 1) / cellSize);
                
                
                for (var x = 0; x < rawCellAmountX; x++)
                {
                    for (var y = 0; y < rawCellAmountY; y++)
                    {
                        var position = new Vector3(
                                           _levelSize + cellSize * x, 
                                           .5f,
                                           cellSize * y - _levelScriptableObject.defaultLevelWith / 2 + cellSize) 
                                       + new Vector3(
                                           Random.Range(-halfCellSize, halfCellSize),
                                           0,
                                           Random.Range(-halfCellSize, halfCellSize));
                        
                        if(position.z > -size.y / 2 -.25f &&
                           position.z < size.y / 2 +.25f) continue;
                        
                        var scale = new Vector3(
                            GetRandomScaleValue(),
                            GetRandomScaleValue(),
                            GetRandomScaleValue());
                        
                        var treeObject = _treePool.GetObject();
                        treeObject.position = position;
                        treeObject.localScale = scale;
                        
                        if (position.z > -_levelScriptableObject.gameLevelSizeWith &&
                            position.z < _levelScriptableObject.gameLevelSizeWith)
                        {
                            var tree = new TreeData(treeObject);
                            _levelTree.Add(tree);
                        }
                        else
                        {
                            _levelBackgroundTree.Add(treeObject);
                        }
                    }
                }

                if (!isCheckPoint)
                {
                    //todo ...
                    var checkPoint = Object.Instantiate(_levelScriptableObject.checkPoint);
                    checkPoint.transform.position = new Vector3(
                        _levelSize + size.x / 2 - .5f, .01f, 0);  
                }
                else
                {
                    if(_door == null)
                    {
                        _door = Object.Instantiate(_levelScriptableObject.door);
                        _door.transform.position = new Vector3(
                            _levelSize + size.x / 2 - .5f, 1f, 0);

                        _door.Set(false);
                    }
                }

                _levelSize += size.x;
            }
        }

        public float GetLevelSize() => _levelSize;

        public float CalcBullet(Transform bullet)
        {
            var bulletScale = bullet.localScale.x;
            var minCollisionPos = float.MaxValue;

            TreeData treeTemp = null;
            foreach (var tree in _levelTree)
            {
                if (!tree.IsEnable) continue;

                var treePos = tree.Transform.position;
                if(minCollisionPos < treePos.x) continue;
                ;
                var halfTreeScale = tree.Transform.localScale.x / 2;

                if (treePos.z + halfTreeScale > -bulletScale / 2 &&
                    treePos.z - halfTreeScale < bulletScale / 2)
                {
                    minCollisionPos = Mathf.Min(
                        treePos.x - halfTreeScale, 
                        minCollisionPos);
                }
            }

            return Mathf.Min(minCollisionPos, _levelSize);
        }

        public void CalcBulletExplosion(Transform bullet, Action onComplete)
        {
            var result = new List<TreeData>();
            var bulletScale = bullet.localScale.x;
            var bulletPos = bullet.position.x;

            foreach (var tree in _levelTree)
            {
                if(!tree.IsEnable) continue;

                var treePos = tree.Transform.position;
                var halfTreeScale = tree.Transform.localScale / 2;
                
                treePos.y = 0;

                if (treePos.x - halfTreeScale.x < bulletPos + bulletScale &&
                    treePos.x + halfTreeScale.x > bulletPos - bulletScale &&
                    treePos.z - halfTreeScale.z < bulletScale&& 
                    treePos.z + halfTreeScale.z > -bulletScale)
                {
                    result.Add(tree);
                }
            }
            
            _behaviour.StartCoroutine(ExplosionTree(result, onComplete));
        }

        private IEnumerator ExplosionTree(List<TreeData> treeList, Action onComplete)
        {
            var t = 0f;
            while (t < _levelScriptableObject.treeAnimTimeSeconds)
            {
                t += Time.deltaTime;
                
                var color = Color.Lerp(
                    _levelScriptableObject.treeFirstColor,
                    _levelScriptableObject.treeSecondColor,
                    t / _levelScriptableObject.treeAnimTimeSeconds);
                
                foreach (var tree in treeList)
                {
                    tree.Material.color = color;
                }

                yield return 0;
            }
            
            var particles = new List<ParticleSystem>();

            foreach (var tree in treeList)
            {
                tree.Transform.gameObject.SetActive(false);
                tree.IsEnable = false;
                
                var particle = _treeParticlePool.GetObject();
                particle.gameObject.SetActive(true);
                particle.transform.position = tree.Transform.position;
                particles.Add(particle);
            }
            
            _hapticComponent.Vibrate(Haptic.Light);

            yield return new WaitForSeconds(2);
            
            foreach (var particle in particles)
            {
                _treeParticlePool.ReturnObject(particle);
                particle.gameObject.SetActive(false);
            }
            
            onComplete?.Invoke();
        }

        public bool CheckLevelComplete(float playerScale)
        {
            var halfPlayerScale = playerScale / 2;
            
            foreach (var tree in _levelTree)
            {
                if(!tree.IsEnable) continue;

                var treeTransform = tree.Transform;
                
                var treePosition = treeTransform.position.z;
                var halfTreeScale = treeTransform.localScale.x / 2;

                if (treePosition + halfTreeScale > -halfPlayerScale &&
                    treePosition - halfTreeScale < halfPlayerScale) return false;
            }

            return true;
        }

        public Door GetDoor() => _door;

        public void ResetLevel()
        {
            foreach (var tree in _levelTree)
            {
                if(tree.IsEnable) continue;
                
                tree.Material.color = _levelScriptableObject.treeFirstColor;
                tree.Transform.gameObject.SetActive(true);
                tree.IsEnable = true;
            }
            
            _door.Set(false);
        }
    }
}