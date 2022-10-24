using TMPro;
using UI.InGame;
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

        public void CreateText(string text, float displayTime = 0, int id = 0)
        {
            GameObject inst;

            if (id == 0)
                inst = Instantiate(this.textPrefab, this.transform);
            else
            {
                inst = this.GetTextWithId(id);
                if (inst == null)
                    inst = Instantiate(this.textPrefab, this.transform);
            }

            inst.GetComponent<TextMeshProUGUI>().SetText(text);
            var gameText = inst.GetComponent<GameText>();

            gameText.CustomId = id;

            if (displayTime > 0)
                Destroy(inst, displayTime);
            else
                gameText.PermanentText = true;
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

        private GameObject GetTextWithId(int id)
        {
            for (var i = 0; i < this.transform.childCount; i++)
            {
                var child = this.transform.GetChild(i).gameObject;
                if (child.GetComponent<GameText>().CustomId == id)
                {
                    return child;
                }
            }

            return null;
        }

        private void HandleLevelCompletedEvent()
        {
            this.RemovePermanentTexts();
            this.CreateText("Fly through a portal to complete the level");
        }
    }
}