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
            this.CreateText("Destroy at least 80% of enemy ships");

            GameManager.Instance.LevelCompletedEvent += this.HandleLevelCompletedEvent;
        }

        private void OnDestroy()
        {
            GameManager.Instance.Texts = null;
            GameManager.Instance.LevelCompletedEvent -= this.HandleLevelCompletedEvent;
        }

        public void CreateText(string text, float displayTime = 0)
        {
            var inst = Instantiate(this.textPrefab, this.transform);
            inst.GetComponent<TextMeshProUGUI>().SetText(text);

            if (displayTime > 0)
                Destroy(inst, displayTime);
        }

        public void RemoveAllTexts()
        {
            while (this.transform.childCount > 0)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            }
        }

        private void HandleLevelCompletedEvent()
        {
            this.RemoveAllTexts();
            this.CreateText("Fly through a portal to complete the level");
        }
    }
}