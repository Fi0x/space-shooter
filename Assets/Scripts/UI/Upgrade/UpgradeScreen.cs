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
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Text freePointTextField;
        
        private readonly List<GameObject> upgradeList = new List<GameObject>();
        private static GameObject _upgradeMenu;
        private static Scrollbar _scrollbar;

        private void Start()
        {
            _upgradeMenu = this.mainGameObject;
            _upgradeMenu.SetActive(false);
            _scrollbar = this.scrollbar;
            
            foreach (var upgrade in UpgradeHandler.GetAllUpgrades())
            {
                var newPrefab = Instantiate(this.upgradePrefab, this.transform);
                
                var (txtName, txtValue) = GetTextComponents(newPrefab);
                txtName.text = Upgrades.GetDisplayName(upgrade.Key);
                txtValue.text = upgrade.Value.ToString();

                var (btnDecrease, btnIncrease) = GetUpgradeButtonComponents(newPrefab);
                btnDecrease.Type = upgrade.Key;
                btnIncrease.Type = upgrade.Key;
                
                this.upgradeList.Add(newPrefab);
            }
            
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10 + 50 * this.upgradeList.Count);
            this.scrollbar.value = 1;

            UpgradeButton.UpgradePurchasedEvent += (sender, args) =>
            {
                this.freePointTextField.text = "Free Points: " + UpgradeHandler.FreeUpgradePoints;
                
                this.upgradeList.ForEach(entry =>
                {
                    var (nameText, valueText) = GetTextComponents(entry);
                    var enumName = Upgrades.GetTypeFromDisplayName(nameText.text);
                    valueText.text = UpgradeHandler.GetSpecificUpgrade(enumName).ToString();
                });
            };
        }

        public static void ShowUpgradeScreen()
        {
            GameManager.IsGamePaused = true;
            _upgradeMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _scrollbar.value = 1;
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
    }
}