using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject anchorPoint;

    private static TextManager _instance;

    public static TextManager Instance
    {
        get
        {
            Debug.Log("Instance accessed");
            if (_instance is null)
            {
                Debug.LogWarning("Instance is null");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        Debug.Log("Awoken");
        if (_instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
            Debug.Log("Instance saved");
        }
        else
        {
            Debug.Log("Destroying object");
            Destroy(gameObject);
        }
    }
    
    public void ShowText(string text, int displayTime)
    {
        var inst = Instantiate(this.textPrefab);
        inst.transform.parent = this.anchorPoint.transform;
        inst.GetComponent<TextMeshPro>().text = text;
        //TODO: Delete entry after display time is over
    }

    public void CleanUp()
    {
        while (this.transform.childCount > 0)
        {
            Destroy(this.transform.GetChild(0));
        }
    }
}
