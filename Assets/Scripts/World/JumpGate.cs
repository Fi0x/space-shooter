using Manager;
using Stats;
using UI.Upgrade;
using UnityEngine;

public class JumpGate : MonoBehaviour
{
    [SerializeField] private string transitionSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            StatCollector.UpdateGeneralStat("Levels Completed", 1);
            GameManager.Instance.ShowUpgradeScreen();
        }
    }
}