using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpGate : MonoBehaviour
{
    [SerializeField] private string transitionSceneName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            LevelTransitionMenu.ShowUpgradeScreen();
        }
    }
}