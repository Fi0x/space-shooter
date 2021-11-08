using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private GameObject player;

        public GameObject Player => this.player;

        public EnemyManager EnemyManager => this.enemyManager;

        public void NotifyAboutNewPlayerInstance(GameObject player)
        {
            this.player = player;
        }


        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogWarning("Instance is null");
                }

                return instance;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }

        private void Start()
        {
            foreach (var _ in Enumerable.Range(0, 0))
            {
                var pos = Random.onUnitSphere * 100;
                this.enemyManager.SpawnNewEnemy(pos);
            }
        }
    }
}