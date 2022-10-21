using TMPro;
using UI.InGame;
using UnityEngine;

namespace Manager
{
    public class TextManager : MonoBehaviour
    {
        [SerializeField] private GameObject textPrefab;
        private int nextID = 1;

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
            inst.GetComponent<GameText>().CustomId = this.nextID;

            if (displayTime > 0)
                Destroy(inst, displayTime);
            else
                inst.GetComponent<GameText>().PermanentText = true;

            this.nextID++;
        }

        private void RemovePermanentTexts()
        {
            for (var i = 0; i < this.transform.childCount; i++)
            {
                var child = this.transform.GetChild(i).gameObject;
                if (child.GetComponent<GameText>().PermanentText)
                {
                    DestroyImmediate(child);
                    i--;
                }
            }
        }

        private void HandleLevelCompletedEvent()
        {
            this.RemovePermanentTexts();
            this.CreateText("Fly through a portal to complete the level");
        }
    }
}