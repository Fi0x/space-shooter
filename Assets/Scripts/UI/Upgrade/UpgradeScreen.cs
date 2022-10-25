using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UpgradeSystem;
using Cursor = UnityEngine.Cursor;

namespace UI.Upgrade
{
    public class UpgradeScreen : MonoBehaviour
    {
        [SerializeField] private GameObject scrollAreaGameObject;
        [SerializeField] private GameObject upgradePrefab;
        [SerializeField] private GameObject subtopicPrefab;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private TextMeshProUGUI freePointTextField;
        [SerializeField] private int upgradesToShow = 3;

        public UpgradeDataSO upgradeData;
        public UpgradeSpriteLookupSO spriteLookup;

        private Dictionary<UpgradeNames, UpgradeField> fields;
        private int subtopicCount;

        //public static event EventHandler UpgradeScreenShownEvent;

        private void Start()
        {
            fields = new Dictionary<UpgradeNames, UpgradeField>();
            GameManager.Instance.currentUpgradeScreen = this;
            //ExpandUpgradeList();
            GenerateRandomUpgrades();
            gameObject.SetActive(false);
        }
        
        public void ShowUpgradeScreen()
        {
            //ExpandUpgradeList();
            scrollbar.value = 1;
            UpdatePoints();
            UpdateAllFields();
        
            //UpgradeScreenShownEvent?.Invoke(null, null);
        }

        private void UpdateField(UpgradeNames type)
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
        
        public void PurchaseUpgrade(UpgradeNames upgradeName, bool isIncrease)
        {
            var negativeCost = CalculateDecreaseCost(upgradeName);
            var positiveCost = CalculateUpgradeCost(upgradeName);
            upgradeData.freePoints -= isIncrease ? positiveCost : -negativeCost;
            upgradeData.AddPoints(upgradeName, isIncrease ? 1 : -1);
            UpdateAllFields();
            UpdatePoints();
        }

        public int CalculateDecreaseCost(UpgradeNames type)
        {
            return 0; // TODO
        }

        public int CalculateUpgradeCost(UpgradeNames type) => (int)upgradeData.GetNextUpgrade(type).Cost;

        private void GenerateRandomUpgrades()
        {
            var upgradeList = this.upgradeData.GetAllUpgradeable();
            var expandedUpgradeList = this.upgradeData.ExpandUpgradesByWeight(upgradeList);
            var shuffledUpgradeList = UpgradeHelper.FisherYatesCardDeckShuffle(expandedUpgradeList);

            for (var i = 0; i < this.upgradesToShow; i++)
            {
                var nextUpgradeName = shuffledUpgradeList[0];
                this.SpawnUpgradeField(nextUpgradeName);

                while (shuffledUpgradeList.Contains(nextUpgradeName))
                    shuffledUpgradeList.Remove(nextUpgradeName);
            }

            this.UpdateAllFields();
        }

        private void SpawnUpgradeField(UpgradeNames type)
        {
            var newPrefab = Instantiate(this.upgradePrefab, this.scrollAreaGameObject.transform);
            var field = newPrefab.GetComponent<UpgradeField>();
            field.upgradeScreen = this;
            field.type = type;
            field.UpdateField();
            if (fields.ContainsKey(type))
            {
                //Destroy(newPrefab);
                Debug.Log("Already contains " + type);
            }else
                fields.Add(type, field);
        }

        public void RemoveAllFields()
        {
            var toRemove = new List<GameObject>();
            for (int i = 0; i < scrollAreaGameObject.transform.childCount; i++)
            {
                var child = scrollAreaGameObject.transform.GetChild(i);
                toRemove.Add(child.gameObject);
            }
            foreach (var child in toRemove)
            {
                Destroy(child);
            }
            fields.Clear();
        }
    }
}