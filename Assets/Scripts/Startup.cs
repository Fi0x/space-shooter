using Manager;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup
{
    static Startup()
    {
        SceneManager.sceneLoaded += FillScene;
    }

    private static void FillScene(Scene scene, LoadSceneMode sceneMode)
    {
        var overlayMenuPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/OverlayMenu.prefab");
        Object.Instantiate(overlayMenuPrefab);

        var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/Player.prefab");
        var playerObject = Object.Instantiate(playerPrefab);

        var gameManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GameController.prefab");
        var gameManagerScript = Object.Instantiate(gameManagerPrefab).GetComponent<GameManager>();
        
        var audioManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AudioManager.prefab");
        var audioManager = Object.Instantiate(audioManagerPrefab);
        
        gameManagerScript.NotifyAboutNewPlayerInstance(playerObject);
    }
}