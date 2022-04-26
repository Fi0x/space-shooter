using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager
{
    public class Player : MonoBehaviour
    {
        private InputMap input;
        
        private void OnEnable()
        {
            input = new InputMap();
            input.Player.Enable();
            input.Player.Pause.performed += OnPause;
        }

        private void OnDisable()
        {
            input.Player.Disable();
            input.Player.Pause.performed -= OnPause;
        }

        private void OnPause(InputAction.CallbackContext ctx)
        {
            GameManager.Instance.ChangePauseState();
        }
        
        private void Awake()
        {
            // if (instance == null)
            // {
            //     instance = this;
            //     DontDestroyOnLoad(gameObject);
            // }
            // else
            // {
            //     instance.transform.position = transform.position;
            //     Destroy(gameObject);
            //     return;
            // }
            
            //Debug.Log("Notified player");
            GameManager.Instance.NotifyAboutNewPlayerInstance(gameObject);
        }
    }
}