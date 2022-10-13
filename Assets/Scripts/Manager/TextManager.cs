using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class TextManager : MonoBehaviour
    {
        [SerializeField] private GameObject textPrefab;

        private void Start()
        {
            GameManager.Instance.Texts = this;
            this.ShowText("Test");
            this.ShowText("Timed Test", 2);
        }

        public void ShowText(string text, float displayTime = 0)
        {
            var inst = Instantiate(this.textPrefab, this.transform);
            inst.GetComponent<TextMeshProUGUI>().SetText(text);
            
            if(displayTime > 0)
                Destroy(inst, displayTime);
        }
    }
}