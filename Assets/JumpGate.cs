using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class JumpGate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            GameManager.Instance.LoadNextLevel();
        }
    }
}
