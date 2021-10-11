using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] public List<GameObject> enemies = new List<GameObject>();

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
}