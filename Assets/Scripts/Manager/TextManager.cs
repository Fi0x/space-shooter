using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject anchorPoint;

    public static TextManager Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
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
