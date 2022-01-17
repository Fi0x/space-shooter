using Manager;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
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
        
        gameManagerScript.NotifyAboutNewPlayerInstance(playerObject);
    }
}