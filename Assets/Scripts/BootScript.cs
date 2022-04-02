using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootScript : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private bool loadStarted;
    
    private void Start()
    {
        //StartLoading();
    }

    public void StartLoading()
    {
        GameManager.Instance.LoadNextLevel();
        //LoadFirstScene();
    }

    private void LoadFirstScene()
    {
        var playerObject = Instantiate(this.playerPrefab);
        GameManager.Instance.NotifyAboutNewPlayerInstance(playerObject);

        Destroy(this.gameObject);
    }
}
