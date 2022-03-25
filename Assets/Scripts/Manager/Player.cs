using System;
using UnityEngine;

namespace Manager
{
    public class Player : MonoBehaviour
    {
        public static Player instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                instance.transform.position = transform.position;
                Destroy(gameObject);
                return;
            }
            
            //Debug.Log("Notified player");
            GameManager.Instance.NotifyAboutNewPlayerInstance(gameObject);
        }
    }
}