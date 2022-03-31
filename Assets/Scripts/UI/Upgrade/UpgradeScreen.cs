using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using UpgradeSystem;

namespace UI.Upgrade
{
    public class UpgradeScreen : MonoBehaviour
    {
        [SerializeField] private GameObject mainGameObject;
        [SerializeField] private GameObject upgradePrefab;
        [SerializeField] private GameObject subtopicPrefab;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Text freePointTextField;

        private static UpgradeScreen _upgradeScreen;
        private static GameObject _upgradeMenu;
        private static Scrollbar _scrollbar;
        private static Text _freePointTextField;
        
        private readonly Dictionary<string, GameObject> upgradeList = new Dictionary<string, GameObject>();
        private int subtopicCount;

        public static event EventHandler UpgradeScreenShownEvent;

        private void Start()
        {
            _upgradeScreen = this;
            _upgradeMenu = this.mainGameObject;
            _upgradeMenu.SetActive(false);
            _scrollbar = this.scrollbar;
            _freePointTextField = this.freePointTextField;

            UpgradeButton.UpgradePurchasedEvent += (sender, args) =>
            {
                this.freePointTextField.text = "Free Points: " + UpgradeHandler.FreeUpgradePoints;

                foreach (var upgradeEntry in this.upgradeList)
                {
                    var (_, valueText) = GetTextComponents(upgradeEntry.Value);
                    valueText.text = UpgradeHandler.GetSpecificUpgrade((Enum) Enum.Parse(typeof(Upgrades.UpgradeNames), upgradeEntry.Key)).ToString();
                }
            };

            this.ExpandUpgradeList();
        }

        public static void ShowUpgradeScreen()
        {
            _upgradeScreen.ExpandUpgradeList();
            _freePointTextField.text = "Free Points: " + UpgradeHandler.FreeUpgradePoints;
            
            GameManager.IsGamePaused = true;
            _upgradeMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _scrollbar.value = 1;

            UpgradeScreenShownEvent?.Invoke(null, null);
        }

        public static void LoadNextLevel()
        {
            GameManager.IsGamePaused = false;
            _upgradeMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.Instance.LoadNextLevel();
        }

        private static (Text name, Text value) GetTextComponents(GameObject upgradeObject)
        {
            var textComponents = upgradeObject.GetComponentsInChildren<Text>(); 
            var nameText = textComponents.Where(c => c.gameObject.name.Equals("Name"));
            var valueText = textComponents.Where(c => c.gameObject.name.Equals("Value"));

            return (nameText.First(), valueText.First());
        }

        private static (UpgradeButton decrease, UpgradeButton increase) GetUpgradeButtonComponents(GameObject upgradeObject)
        {
            var buttonComponents = upgradeObject.GetComponentsInChildren<UpgradeButton>();
            var decrease = buttonComponents.First(c => c.gameObject.name.Equals("Decrease"));
            var increase = buttonComponents.First(c => c.gameObject.name.Equals("Increase"));

            return (decrease, increase);
        }

        private void ExpandUpgradeList()
        {
            var currentCategory = "";
            foreach (var upgrade in UpgradeHandler.GetAllUpgrades())
            {
                if(this.upgradeList.ContainsKey(upgrade.Key.ToString()))
                    continue;

                var category = UpgradeHandler.GetUpgradeCategory(upgrade.Key);
                if (!currentCategory.Equals(category))
                {
                    currentCategory = category;
                    var newSubtopic = Instantiate(this.subtopicPrefab, this.transform);
                    
                    var textComponents = newSubtopic.GetComponentsInChildren<Text>(); 
                    var nameText = textComponents.First(c => c.gameObject.name.Equals("Name"));
                    nameText.text = currentCategory;

                    this.subtopicCount++;
                }
                    
                var newPrefab = Instantiate(this.upgradePrefab, this.transform);
                
                var (txtName, txtValue) = GetTextComponents(newPrefab);
                txtName.text = Upgrades.GetDisplayName(upgrade.Key);
                txtValue.text = upgrade.Value.ToString();

                var (btnDecrease, btnIncrease) = GetUpgradeButtonComponents(newPrefab);
                btnDecrease.Type = upgrade.Key;
                btnIncrease.Type = upgrade.Key;
                
                this.upgradeList.Add(upgrade.Key.ToString(), newPrefab);
            }
            
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10 + 50 * (this.upgradeList.Count + this.subtopicCount));
            this.scrollbar.value = 1;
        }
    }
}