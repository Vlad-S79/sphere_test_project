using Game.Level.Common;
using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = "level_scriptable_object", menuName = "Game/Level", order = 1)]
    public class LevelScriptableObject : ScriptableObject
    {
        public GameObject ground;
        public GameObject tree;

        public GameObject checkPoint;
        public Door door;

        [Space]
        public int pointAmount;
        public float sizeLevelToPoint;

        [Space] 
        public Vector2 checkPointSize;
        public Vector2 doorSize;

        [Space]
        public float defaultLevelWith;
        public float cellTreeSize;

        [Space] 
        public float groundTextureScaler;
        public float offsetBeforeLevel;
        public float offsetAfterLevel;

        [Space] 
        public float gameLevelSizeWith;

        [Space] 
        public ParticleSystem treeParticle;
        public float treeAnimTimeSeconds;
        public Color treeFirstColor;
        public Color treeSecondColor;
    }
}