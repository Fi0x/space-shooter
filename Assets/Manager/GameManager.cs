using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private GameObject player;
        [SerializeField] private int enemySpawnRange = 300;
        [SerializeField] private int enemyCount = 5;

        public GameObject Player => this.player;

        public EnemyManager EnemyManager => this.enemyManager;

        public void NotifyAboutNewPlayerInstance(GameObject newPlayer)
        {
            this.player = newPlayer;
        }


        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning("Instance is null");
                }

                return _instance;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }

        private void Start()
        {
            foreach (var _ in Enumerable.Range(0, this.enemyCount))
            {
                var pos = Random.onUnitSphere * this.enemySpawnRange;
                this.enemyManager.SpawnNewEnemy(pos);
            }
        }
    }
}