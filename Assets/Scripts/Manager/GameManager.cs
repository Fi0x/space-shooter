using Components;
using LevelManagement;
using Ship;
using Stats;
using UI;
using UI.GameOver;
using UI.Upgrade;
using UnityEngine;
using UnityEngine.SceneManagement;
using UpgradeSystem;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        //[SerializeField] private EnemyManager enemyManager;
        [SerializeField] public UpgradeScreen currentUpgradeScreen;
        [SerializeField] private LevelFlowSO levelFlow;
        [SerializeField] public UpgradeDataSO playerUpgrades;
        [SerializeField] private int playerDefaultHealth = 1000;

        private GameObject _player;

        public GameObject Player
        {
            get
            {
                if (_player == null) return null;
                return _player;
            }
            private set
            {
                _player = value;
            }
        }

        //public EnemyManager EnemyManager => this.enemyManager;

        public static bool IsGamePaused { get; set; } = false;

        public float difficulty = 1f;

        [SerializeField, ReadOnlyInspector]private int levelIndex = 0;

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
            if (_instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                _instance = this;
                //this.Player = GameObject.Find("Player");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            playerUpgrades.ResetData();
        }

        public void ResetGame()
        {
            levelIndex = -1;
            StatCollector.ResetStats();
            playerUpgrades.ResetData();
        }

        public void LoadNextLevel()
        {
            // this.EnemyManager.RemoveAllEnemies();
            // this.LevelBuilder.LoadRandomLevel();
            // this.flockSpawner.SpawnFlocks();
            //this.SpawnPlayer();
            var levelName = levelFlow.GetNextScene(levelIndex);
            SceneManager.LoadScene(levelName);
            levelIndex++;
            AddDifficulty();
        }
        
        public void ShowUpgradeScreen()
        {
            currentUpgradeScreen.gameObject.SetActive(true);
            currentUpgradeScreen.ShowUpgradeScreen();
            
            IsGamePaused = true;
            currentUpgradeScreen.gameObject.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        
            //_scrollbar.value = 1;
        
            //UpgradeScreenShownEvent?.Invoke(null, null);
        }

        private void AddDifficulty()
        {
            difficulty = 1f + Mathf.Log(levelIndex) * 3f;
        }

        public void ChangePauseState()
        {
            if(IsGamePaused) OverlayMenu.Resume();
            else OverlayMenu.Pause();
        }

        public void GameOver()
        {
            GameOverScreen.ShowGameOverScreen();
        }

        private void SpawnPlayer()
        {
            this.Player.transform.position = new Vector3(0, 0, 0);
            this.Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.Player.GetComponent<ShipMovementHandler>().SetNewTargetSpeed(0);
            var playerHealth = this.Player.GetComponent<Health>();
            playerHealth.MaxHealth = this.playerDefaultHealth;
            playerHealth.CurrentHealth = playerHealth.MaxHealth;
        }
    }
}