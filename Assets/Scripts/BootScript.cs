using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootScript : MonoBehaviour
{
    [SerializeField] private string levelSceneName;
    [SerializeField] private GameObject overlayMenuPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject gameControllerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;

    private bool loadStarted;
    
    private void Start()
    {
        if(this.loadStarted)
            return;
        
        this.loadStarted = true;
        
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene(this.levelSceneName);
        
        SceneManager.sceneLoaded += (arg0, mode) =>
        {
            if (arg0.name.Equals(this.levelSceneName))
                this.LoadFirstScene();
        };
    }

    private void LoadFirstScene()
    {
        Instantiate(this.overlayMenuPrefab);
        var playerObject = Instantiate(this.playerPrefab);
        var gameManagerScript = Instantiate(this.gameControllerPrefab).GetComponent<GameManager>();
        
        Instantiate(this.audioManagerPrefab);
        
        gameManagerScript.NotifyAboutNewPlayerInstance(playerObject);

        Destroy(this.gameObject);
    }
}
