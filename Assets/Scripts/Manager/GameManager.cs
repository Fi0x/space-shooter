using Components;
using Enemy;
using Ship;
using Ship.Movement;
using Stats;
using UI;
using UI.GameOver;
using UnityEngine;
using UpgradeSystem;
using World;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private LevelBuilder levelBuilder;
        [SerializeField] private FlockSpawner flockSpawner;
        [SerializeField] private int playerDefaultHealth = 1000;

        public GameObject Player { get; private set; }
        
        public TargetableManager TargetableManager { get; private set; }

        public EnemyManager EnemyManager => this.enemyManager;

        public LevelBuilder LevelBuilder => this.levelBuilder;

        public static bool IsGamePaused { get; set; } = false;

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
            this.TargetableManager ??= new TargetableManager();
            this.Player = GameObject.Find("Player");
        }

        private void Start()
        {
            this.LoadNextLevel();
        }

        public static void ResetGame()
        {
            level = 0;
            StatCollector.ResetStats();
            UpgradeHandler.Reset();
        }

        public void LoadNextLevel()
        {
            level++;
            this.EnemyManager.RemoveAllEnemies();
            this.LevelBuilder.LoadRandomLevel();
            this.flockSpawner.SpawnFlocks();
            this.SpawnPlayer();
        }

        public static void ChangePauseState()
        {
            if(IsGamePaused) OverlayMenu.Resume();
            else OverlayMenu.Pause();
        }

        public static void GameOver()
        {
            GameOverScreen.ShowGameOverScreen();
        }

        private void SpawnPlayer()
        {
            this.Player.transform.position = new Vector3(0, 0, 0);
            this.Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.Player.GetComponent<PlayerShipMovementHandler>().SetNewTargetSpeed(0);
            var playerHealth = this.Player.GetComponent<Health>();
            playerHealth.MaxHealth = this.playerDefaultHealth;
            playerHealth.CurrentHealth = playerHealth.MaxHealth;
        }
    }
}