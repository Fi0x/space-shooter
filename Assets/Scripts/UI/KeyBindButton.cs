using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class KeyBindButton : MonoBehaviour
    {
        private void Start()
        {
            var text = InputManager.GetKeyCodeForName(this.gameObject.name);
            this.gameObject.GetComponentInChildren<Text>().text = text;

            var button = this.gameObject.GetComponent<Button>();
            button.onClick.AddListener(() => InputManager.Instance.NextKeyToBind(button));
        }
    }
}
