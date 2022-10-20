using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "new Enemy Settings", menuName = "Enemy/Settings", order = 0)]
    public class EnemyAISO : ScriptableObject
    {
        [Header("Ship")]
        public float radius;
        public float sightDistance;
        public LayerMask collisionMask;

        [Header("Movement")] 
        public float maxSpeed;
        public float tiltSpeed;
        public float rollSpeed;
        public float patrolRadius;

        [Header("Combat")]
        public GameObject projectilePrefab;
        public float attackSpeed;
        public float attackAngle = 5f;
    }
}