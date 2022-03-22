using System;
using UnityEngine;

namespace Manager
{
    public class Player : MonoBehaviour
    {
        private void Awake()
        {
            //Debug.Log("Notified player");
            GameManager.Instance.NotifyAboutNewPlayerInstance(gameObject);
        }
    }
}