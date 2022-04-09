using System;
using Manager;
using Stats;
using UI.Upgrade;
using UnityEngine;

public class JumpGate : MonoBehaviour
{
    [SerializeField] private string transitionSceneName;
    [SerializeField] private GameObject animationPlane;

    private void Start()
    {
        this.animationPlane.SetActive(false);
        GameManager.Instance.LevelCompletedEvent += (sender, args) => this.animationPlane.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            StatCollector.UpdateGeneralStat("Levels Completed", 1);
            GameManager.Instance.ShowUpgradeScreen();
        }
    }
}