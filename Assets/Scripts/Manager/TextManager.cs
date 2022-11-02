using TMPro;
using UI.InGame;
using UnityEngine;

namespace Manager
{
    public class TextManager : MonoBehaviour
    {
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private GameObject barPrefab;

        private void Start()
        {
            this.CreateText("Destroy at least 80% of enemy ships", 0, "mainGoal");
            GameManager.Instance.Texts = this;

            GameManager.Instance.LevelCompletedEvent += this.HandleLevelCompletedEvent;
        }

        private void OnDestroy()
        {
            GameManager.Instance.Texts = null;
            GameManager.Instance.LevelCompletedEvent -= this.HandleLevelCompletedEvent;
        }

        public void CreateText(string text, float displayTime = 0, string id = "", float barValue = -1)
        {
            GameObject inst;
            var correctPrefab = barValue >= 0 ? this.barPrefab : this.textPrefab;

            if (id.Equals(""))
                inst = Instantiate(correctPrefab, this.transform);
            else
            {
                inst = this.GetTextWithId(id);
                if (inst == null)
                    inst = Instantiate(correctPrefab, this.transform);
            }

            inst.GetComponent<TextMeshProUGUI>().SetText(text);
            var gameText = inst.GetComponent<GameText>();

            gameText.CustomId = id;

            if (displayTime > 0)
                Destroy(inst, displayTime);
            else
                gameText.PermanentText = true;

            if (barValue >= 0)
                gameText.BarPercentage = barValue;
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

        private GameObject GetTextWithId(string id)
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