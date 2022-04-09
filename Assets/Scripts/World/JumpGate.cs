using Manager;
using Stats;
using UnityEngine;

public class JumpGate : MonoBehaviour
{
    [SerializeField] private string transitionSceneName;
    [SerializeField] private GameObject animationPlane;

    private bool isActivated;

    private void Start()
    {
        this.animationPlane.SetActive(false);
        GameManager.Instance.LevelCompletedEvent += (sender, args) =>
        {
            this.isActivated = true;
            this.animationPlane.SetActive(true);
        };
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