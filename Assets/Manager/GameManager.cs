using System.Linq;
using UnityEngine;
using World;
using Random = UnityEngine.Random;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private GameObject player;
        [SerializeField] private int enemySpawnRange = 300;
        [SerializeField] private int enemyCount = 5;

        public GameObject Player => player;

        public EnemyManager EnemyManager => enemyManager;
        
        private LevelBuilder LevelBuilder { get; set; }
        
        public static bool IsGamePaused { get; set; }

        public void NotifyAboutNewPlayerInstance(GameObject newPlayer)
        {
            this.player = newPlayer;
        }

        private static GameManager _instance;
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
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }

        private void Start()
        {
            LevelBuilder = gameObject.GetComponent<LevelBuilder>();
            LoadNextLevel();
        }

        public void LoadNextLevel()
        {
            EnemyManager.RemoveAllEnemies();
            LevelBuilder.LoadRandomLevel();
            SpawnEnemies();
        }

        public static void ChangePauseState()
        {
            if(IsGamePaused) PauseMenu.Resume();
            else PauseMenu.Pause();
        }

        private void SpawnEnemies()
        {
            foreach (var _ in Enumerable.Range(0, enemyCount))
            {
                var pos = Random.onUnitSphere * enemySpawnRange;
                enemyManager.SpawnNewEnemy(pos);
            }
        }
    }
}