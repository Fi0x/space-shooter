using System.Linq;
using Components;
using Ship;
using UI;
using UnityEngine;
using World;
using Random = UnityEngine.Random;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private GameObject player;
        [SerializeField] private int playerDefaultHealth = 1000;
        [SerializeField] private int enemySpawnRange = 300;
        [SerializeField] private int swarmCount = 1;

        [SerializeField] private BoidController boidController;

        public GameObject Player => this.player;

        public EnemyManager EnemyManager => this.enemyManager;
        
        public LevelBuilder LevelBuilder { get; private set; }
        
        public static bool IsGamePaused { get; set; }

        private static int level;

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
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
            this.player = GameObject.Find("Player");
        }

        private void Start()
        {
            this.LevelBuilder = this.gameObject.GetComponent<LevelBuilder>();
            this.LoadNextLevel();
        }

        public void LoadNextLevel()
        {
            level++;
            this.EnemyManager.RemoveAllEnemies();
            this.LevelBuilder.LoadRandomLevel();
            this.SpawnEnemies();
            this.SpawnPlayer();
        }

        public static void ChangePauseState()
        {
            if(IsGamePaused) OverlayMenu.Resume();
            else OverlayMenu.Pause();
        }

        public static void GameOver()
        {
            //TODO: Display Game-over screen
        }

        private void SpawnEnemies()
        {
            Vector3[] pos = new Vector3[this.swarmCount];

            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = player.transform.position + Random.onUnitSphere * enemySpawnRange;
            }

            //boidController.InitializeBoids(this.swarmCount, pos);

            //this.enemyManager.SpawnNewEnemy(pos);
                
            
        }

        private void SpawnPlayer()
        {
            this.player.transform.position = new Vector3(0, 0, 0);
            this.player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.player.GetComponent<ShipMovementHandler>().desiredSpeed = 0;
            var playerHealth = this.player.GetComponent<Health>();
            playerHealth.MaxHealth = this.playerDefaultHealth;
            playerHealth.CurrentHealth = this.playerDefaultHealth;
        }
    }
}