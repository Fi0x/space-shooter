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

        private void Awake()
        {
            BuildLevel();
        }

        private void Start()
        {
            SpawnEnemies();
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