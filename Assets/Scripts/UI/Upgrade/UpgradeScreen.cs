using System.Collections.Generic;
using System.Linq;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UpgradeSystem;

namespace UI.Upgrade
{
    public class UpgradeScreen : MonoBehaviour
    {
        [SerializeField] private GameObject scrollAreaGameObject;
        [SerializeField] private GameObject upgradePrefab;
        [SerializeField] private GameObject subtopicPrefab;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Text freePointTextField;

        public UpgradeDataSO upgradeData;

        private Dictionary<UpgradeNames, UpgradeField> fields;
        private int subtopicCount;

        //public static event EventHandler UpgradeScreenShownEvent;

        private void Start()
        {
            fields = new Dictionary<UpgradeNames, UpgradeField>();
            GameManager.Instance.currentUpgradeScreen = this;
            this.ExpandUpgradeList();
            gameObject.SetActive(false);
        }
        
        public void ShowUpgradeScreen()
        {
            ExpandUpgradeList();
            scrollbar.value = 1;
            UpdatePoints();
            UpdateAllFields();
        
            //UpgradeScreenShownEvent?.Invoke(null, null);
        }

        private void UpdateFields(UpgradeNames type)
        {
            if(fields.TryGetValue(type, out UpgradeField field))
            {
                field.UpdateField();
            }
        }

        private void UpdateAllFields()
        {
            foreach (var field in fields)
            {
                field.Value.UpdateField();
            }
        }

        private void UpdatePoints()
        {
            freePointTextField.text = "Free Points:" + upgradeData.freePoints;
        }

        public void LoadNextLevel()
        {
            GameManager.IsGamePaused = false;
            gameObject.SetActive(false);
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.Instance.LoadNextLevel();
        }

        // private static (Text name, Text value) GetTextComponents(GameObject upgradeObject)
        // {
        //     var textComponents = upgradeObject.GetComponentsInChildren<Text>(); 
        //     var nameText = textComponents.Where(c => c.gameObject.name.Equals("Name"));
        //     var valueText = textComponents.Where(c => c.gameObject.name.Equals("Value"));
        //
        //     return (nameText.First(), valueText.First());
        // }
        //
        // private static (UpgradeButton decrease, UpgradeButton increase) GetUpgradeButtonComponents(GameObject upgradeObject)
        // {
        //     var buttonComponents = upgradeObject.GetComponentsInChildren<UpgradeButton>();
        //     var decrease = buttonComponents.First(c => c.gameObject.name.Equals("Decrease"));
        //     var increase = buttonComponents.First(c => c.gameObject.name.Equals("Increase"));
        //
        //     return (decrease, increase);
        // }
        
        public void PurchaseUpgrade(UpgradeNames upgradeName, int valueChange)
        {
            upgradeData.freePoints -= valueChange;
            upgradeData.AddPoints(upgradeName, valueChange);
            UpdateFields(upgradeName);
            UpdatePoints();
        }

        private void ExpandUpgradeList()
        {
            var currentCategory = "";
            foreach (var upgrade in upgradeData.upgrades)
            {
                var category = UpgradeHelper.GetUpgradeCategory(upgrade.type);
                if (!currentCategory.Equals(category))
                {
                    currentCategory = category;
                    var newSubtopic = Instantiate(this.subtopicPrefab, this.scrollAreaGameObject.transform);
                    
                    var textComponents = newSubtopic.GetComponentsInChildren<TextMeshProUGUI>(); 
                    var nameText = textComponents.First(c => c.gameObject.name.Equals("Name"));
                    nameText.text = currentCategory;
        
                    this.subtopicCount++;
                }
                    
                var newPrefab = Instantiate(this.upgradePrefab, this.scrollAreaGameObject.transform);
                var field = newPrefab.GetComponent<UpgradeField>();
                field.upgradeScreen = this;
                field.type = upgrade.type;
                field.UpdateField();
                if (fields.ContainsKey(upgrade.type))
                {
                    Destroy(newPrefab);
                }else
                    fields.Add(upgrade.type, field);
            }
            
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10 + 50 * (upgradeData.upgrades.Count + this.subtopicCount));
            this.scrollbar.value = 1;
        }
    }
}