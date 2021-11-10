using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class JumpGate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.LoadNextLevel();
    }
}
