using TMPro;
using UnityEngine;

namespace Manager
{
    public class TextManager : MonoBehaviour
    {
        [SerializeField] private GameObject textPrefab;

        private void Start()
        {
            GameManager.Instance.Texts = this;
            this.ShowText("Test", 0);
        }

        public void ShowText(string text, int displayTime)
        {
            var inst = Instantiate(this.textPrefab, this.transform);
            inst.GetComponent<TextMeshPro>().text = text;
            //TODO: Delete entry after display time is over
        }
    }
}