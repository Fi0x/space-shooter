using System;
using Components;
using HealthSystem;
using Helper;
using LevelManagement;
using Ship.Movement;
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
        [SerializeField] public UpgradeScreen currentUpgradeScreen;
        [SerializeField] private LevelFlowSO levelFlow;
        [SerializeField] public UpgradeDataSO playerUpgrades;
        
        public GameObject Player
        {
            get => player;
            private set => player = value;
        }
        

        public static bool IsGamePaused { get; set; } = false;

        public float difficulty = 1f;

        [SerializeField, ReadOnlyInspector]private int levelIndex = 0;
        [SerializeField, ReadOnlyInspector] private GameObject player;

        public void NotifyAboutNewPlayerInstance(GameObject newPlayer)
        {
            this.Player = newPlayer;
        }

        private static GameManager _instance;
        [SerializeField] private TargetableManagerScriptableObject targetableManager;

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

        public TargetableManagerScriptableObject TargetableManager => this.targetableManager;

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
            SceneManager.LoadScene(SceneManagerUtils.SceneId.Startup.AsInt());
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

        [Obsolete]
        private void SpawnPlayer()
        {
            this.Player.transform.position = new Vector3(0, 0, 0);
            this.Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.Player.GetComponent<PlayerShipMovementHandler>().SetNewTargetSpeed(0);
            var playerHealth = this.Player.GetComponent<Health>();
            playerHealth.MaxHealth = 100;//this.playerDefaultHealth;
            playerHealth.CurrentHealth = playerHealth.MaxHealth;
        }
    }
}