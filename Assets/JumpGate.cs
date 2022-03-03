using UI.Upgrade;
using UnityEngine;

public class JumpGate : MonoBehaviour
{
    [SerializeField] private string transitionSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            StatCollector.IntStats[StatCollector.StatValues.LevelsCompleted]++;
            UpgradeScreen.ShowUpgradeScreen();
        }
    }
}