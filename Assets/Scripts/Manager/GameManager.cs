using System.Linq;
using Components;
using Enemy;
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
        [SerializeField] private LevelBuilder levelBuilder;
        [SerializeField] private int playerDefaultHealth = 1000;
        [SerializeField] private int enemySpawnRange = 300;
        [SerializeField] private int swarmCount = 1;

        [SerializeField] private BoidController boidController;

        public GameObject Player { get; private set; }

        public EnemyManager EnemyManager => this.enemyManager;

        public LevelBuilder LevelBuilder => this.levelBuilder;
        
        public static bool IsGamePaused { get; set; }

        private static int level;

        public void NotifyAboutNewPlayerInstance(GameObject newPlayer)
        {
            this.Player = newPlayer;
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
            this.Player = GameObject.Find("Player");
        }

        private void Start()
        {
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
            var pos = new Vector3[this.swarmCount];

            for (var i = 0; i < pos.Length; i++)
            {
                pos[i] = this.Player.transform.position + Random.onUnitSphere * this.enemySpawnRange;
            }
        }

        private void SpawnPlayer()
        {
            this.Player.transform.position = new Vector3(0, 0, 0);
            this.Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.Player.GetComponent<ShipMovementHandler>().desiredSpeed = 0;
            var playerHealth = this.Player.GetComponent<Health>();
            playerHealth.MaxHealth = this.playerDefaultHealth;
            playerHealth.CurrentHealth = this.playerDefaultHealth;
        }
    }
}