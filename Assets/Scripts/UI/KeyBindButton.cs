using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class KeyBindButton : MonoBehaviour
    {
        private void Start()
        {
            var text = KeyManager.GetKeyCodeForName(this.gameObject.name);
            this.gameObject.GetComponentInChildren<Text>().text = text;
        }
    }
}
