using System;
using Components;
using HealthSystem;
using LevelManagement;
using Ship;
using Ship.Movement;
using Stats;
using UI;
using UI.GameOver;
using UI.Upgrade;
using UnityEngine;
using UnityEngine.SceneManagement;
using UpgradeSystem;
using UpgradeSystem.CostAndGain;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        //[SerializeField] private EnemyManager enemyManager;
        [SerializeField] public UpgradeScreen currentUpgradeScreen;
        [SerializeField] private LevelFlowSO levelFlow;
        [SerializeField] public UpgradeDataSO playerUpgrades;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private int playerDefaultHealth = 1000;

        [Header("TargetableManager")] 
        [SerializeField] private Sprite targetableActive;
        [SerializeField] private Sprite targetableInactive;

        [Header("UpgradeConfig")] [SerializeField]
        private UpgradeSystemCostAndGainLookupScriptableObject upgradeMagnitudeLookupScriptableObjectTable;

        public UpgradeSystemCostAndGainLookupScriptableObject UpgradeMagnitudeLookupScriptableObjectTable => this.upgradeMagnitudeLookupScriptableObjectTable;
        
        public GameObject Player { get; private set; }
        
        public TargetableManager TargetableManager { get; private set; }

        public EnemyManager EnemyManager => this.enemyManager;
        
        public int EnemyLevelCounter { get; set; }
        private int _destroyedEnemiesInLevel;
        public int DestroyedEnemyLevelCounter {
            get
            {
                return this._destroyedEnemiesInLevel;
            }
            set
            {
                this._destroyedEnemiesInLevel = value;
                if(100f / this.EnemyLevelCounter * this._destroyedEnemiesInLevel > 80)
                    this.LevelCompletedEvent?.Invoke(null, null);
            }
        }
        
        public event EventHandler LevelCompletedEvent;//TODO: Also invoke if station gets destroyed
        

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
                this.TargetableManager ??= new TargetableManager(targetableActive, targetableInactive);
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
            if (this.upgradeMagnitudeLookupScriptableObjectTable == null)
            {
                throw new NullReferenceException(nameof(this.upgradeMagnitudeLookupScriptableObjectTable));
            }
        }
        
        private void Update()
        {
            this.TargetableManager?.NotifyAboutUpdate();
        }

        public void ResetGame()
        {
            levelIndex = 0;
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

        public void ReturnToMenu()
        {
            ResetGame();
            SceneManager.LoadScene("Startup");
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
            this.Player.GetComponent<PlayerShipMovementHandler>().SetNewTargetSpeed(0);
            var playerHealth = this.Player.GetComponent<Health>();
            playerHealth.MaxHealth = this.playerDefaultHealth;
            playerHealth.CurrentHealth = playerHealth.MaxHealth;
        }
    }
}