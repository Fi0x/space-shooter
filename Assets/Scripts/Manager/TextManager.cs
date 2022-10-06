using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject anchorPoint;
    public void ShowText(string text, int displayTime)
    {
        var inst = Instantiate(this.textPrefab);
        inst.transform.parent = this.anchorPoint.transform;
        inst.GetComponent<TextMeshPro>().text = text;
        //TODO: Delete entry after display time is over
    }

    private void Start()
    {
        Debug.Log("TEST initialized");
    }
}
