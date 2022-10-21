using System;
using Enemy;
using Manager;
using UnityEngine;
using World;

namespace LevelManagement
{
    public class Level : MonoBehaviour
    {
        [Header("Builders")]
        public LevelBuilder levelBuilder;
        public EnemySpawner enemySpawner;
        public StationBuilder stationBuilder;

        [Header("LevelSettings")]
        public bool spawnStation = false;
        public string musicName = "MainMusic";

        private void Start()
        {
            BuildLevel();
            SpawnEnemies();
            AudioManager.instance.ChangeMusic(musicName);
        }

        private void BuildLevel()
        {
            levelBuilder.LoadRandomLevel();
            stationBuilder.SetDifficulty(GameManager.Instance.difficulty);
            if (!spawnStation)
            {
                Destroy(stationBuilder.controller.gameObject);
                Destroy(stationBuilder);
                return;
            }
            stationBuilder.BuildStation();
        }

        private void SpawnEnemies()
        {
            enemySpawner.SetDifficulty(GameManager.Instance.difficulty);
            enemySpawner.SpawnBasic();
            enemySpawner.SpawnElite();
        }
    }
}