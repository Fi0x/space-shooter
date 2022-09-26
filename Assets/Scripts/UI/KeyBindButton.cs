using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class KeyBindButton : MonoBehaviour
    {
        [SerializeField] private InputActionReference rebindAction;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private TMP_Text keyText;
        [SerializeField] private InputBinding bindingMask;

        public void StartRebind()
        {
            rebindAction.action.Disable();
            keyText.text = "Waiting for input ...";

            rebindAction.action.PerformInteractiveRebinding()
                .WithBindingGroup("KeyboardMouse")
                .WithBindingMask(bindingMask)
                .OnComplete(operation => CompleteRebind())
                .Start();
        }

        private void CompleteRebind()
        {
            
        }
    }
}
