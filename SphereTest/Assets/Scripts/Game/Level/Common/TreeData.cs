using UnityEngine;

namespace Game.Level.Common
{
    public class TreeData
    {
        public Transform Transform;
        public Material Material;
        
        public bool IsEnable;

        public float Infection;

        public TreeData(Transform transform)
        {
            IsEnable = true;

            Transform = transform;
            Material = transform.GetComponent<MeshRenderer>().material;
        }
    }
}