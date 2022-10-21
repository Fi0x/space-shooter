using System;
using Manager;
using Stats;
using UI;
using UnityEngine;

namespace World
{
    public class JumpGate : MonoBehaviour
    {
        [SerializeField] private string transitionSceneName;
        [SerializeField] private GameObject animationPlane;

        [SerializeField, ReadOnlyInspector] private bool isActivated;

        private void OnEnable()
        {
            this.isActivated = false;
            GameManager.Instance.LevelCompletedEvent += this.HandleLevelCompletedEvent;
        }

        private void OnDisable()
        {
            GameManager.Instance.LevelCompletedEvent -= this.HandleLevelCompletedEvent;
        }

        private void HandleLevelCompletedEvent()
        {
            if (this.isActivated)
            {
                Debug.Log("Portal already open");
                return;
            }
            Debug.Log("Opening Portal");
            this.isActivated = true;
            this.animationPlane.SetActive(true);
        }
        

        private void Start()
        {
            this.animationPlane.SetActive(this.isActivated);
        }

        private void Awake()
        {
            this.isActivated = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!this.isActivated)
                return;
        
            if (other.gameObject.layer == 8)
            {
                StatCollector.UpdateGeneralStat("Levels Completed", 1);
                GameManager.Instance.ShowUpgradeScreen();
            }
        }
    }
}