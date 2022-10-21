using System;
using System.Collections.Generic;
using Helper;
using LevelManagement;
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
        [SerializeField] private GameObject textManagerPrefab;
        
        public GameObject Player
        {
            get => player;
            private set => player = value;
        }

        private TextManager textManager;
        public TextManager Texts
        {
            private get => this.textManager;
            set
            {
                this.textManager = value;
                foreach (var (text, ttl) in this.textBuffer)
                {
                    this.textManager.CreateText(text, ttl);
                }
                this.textBuffer.Clear();
            }
        }
        private List<(string text, float ttl)> textBuffer = new List<(string text, float ttl)>();

        public int EnemyLevelCounter
        {
            get => enemyLevelCounter;
            set => enemyLevelCounter = value;
        }

        [SerializeField, ReadOnlyInspector]
        private int destroyedEnemiesInLevel;
        public int DestroyedEnemyLevelCounter {
            get => this.destroyedEnemiesInLevel;
            set
            {
                this.destroyedEnemiesInLevel = value;
                if (value <= 0 || this.EnemyLevelCounter == 0)
                    return;

                if (this.destroyedEnemiesInLevel >= this.EnemyLevelCounter)
                {
                    if (!levelAlreadyCompleted)
                    {
                        this.CompleteLevel();
                        this.levelAlreadyCompleted = true;
                    }
                    return;
                }
                
                var fractionDead = (float)this.destroyedEnemiesInLevel / this.EnemyLevelCounter;
                
                this.CreateNewText((fractionDead * 100f) + "% of enemy ships destroyed", 2);
                
                if(fractionDead >= .8f && !this.levelAlreadyCompleted)
                {
                    this.CompleteLevel();
                    this.levelAlreadyCompleted = true;
                }
            }
        }
        
        public event Action LevelCompletedEvent;
        private bool levelAlreadyCompleted;

        public void CompleteLevel()
        {
            this.LevelCompletedEvent?.Invoke();
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
        [SerializeField, ReadOnlyInspector] private int enemyLevelCounter;

        public static GameManager Instance
        {
            get
            {
                if (_instance is null)
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
            this.EnemyLevelCounter = 0;
            this.DestroyedEnemyLevelCounter = 0;
        }

        public void LoadNextLevel()
        {
            var levelName = levelFlow.GetNextScene(levelIndex);
            SceneManager.LoadScene(levelName);
            this.levelIndex++;
            this.AddDifficulty();
            this.levelAlreadyCompleted = false;
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

        public void CreateNewText(string text, float ttl = 0)
        {
            if(this.Texts == null)
                this.textBuffer.Add((text, ttl));
            else
                this.Texts.CreateText(text, ttl);
        }
    }
}