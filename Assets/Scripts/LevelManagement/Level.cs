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
        public FlockSpawner flockSpawner;
        public StationBuilder stationBuilder;

        [Header("LevelSettings")]
        public bool spawnStation = false;
        public string musicName = "MainMusic";

        private void Awake()
        {
            
        }

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
            if(spawnStation) stationBuilder.BuildStation();
        }

        private void SpawnEnemies()
        {
            flockSpawner.SetDifficulty(GameManager.Instance.difficulty);
            flockSpawner.SpawnFlocks();
        }
    }
}