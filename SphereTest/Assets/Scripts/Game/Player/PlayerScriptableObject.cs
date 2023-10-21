using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "player_scriptable_object", menuName = "Game/Player", order = 1)]
    public class PlayerScriptableObject : ScriptableObject
    {
        public GameObject player;
        public GameObject bullet;
        [Space]
        public GameObject playerPath;

        [Space] 
        public float maxPlayerScale;
        public float minPlayerScale;
        public float minBulletScale;
        public float bulletScaleSpeed;
        public float playerScaleSpeed;
        
        [Space] 
        public float playerStartOffset;
        public float pathOffset;

        [Space] 
        public Vector2 finalJumpSize;
        public float finalJumpTimeSeconds;

        [Space] 
        public float bulletSpeed;

    }
}