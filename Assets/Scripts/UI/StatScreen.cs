using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatScreen : MonoBehaviour
    {
        [SerializeField] private GameObject statPrefab;
        [SerializeField] private GameObject subtopicPrefab;
        [SerializeField] private Scrollbar scrollbar;
        
        private readonly List<GameObject> statList = new List<GameObject>();

        private void OnEnable()
        {
            this.UpdateStatList();
        }

        private void UpdateStatList()
        {
            foreach (var entry in this.statList)
                Destroy(entry);
            this.statList.Clear();

            var score = CalculateHighscore();
            this.CreateEntry(new KeyValuePair<string, float>("Highscore", score));
            
            if(StatCollector.GeneralStats.Count > 0) 
                this.CreateSubtopic("General Stats");
            foreach (var stat in StatCollector.GeneralStats)
                this.CreateEntry(stat);
            
            if(StatCollector.WeaponStats.Count > 0)
                this.CreateSubtopic("Weapon Stats");
            foreach (var stat in StatCollector.WeaponStats)
                this.CreateEntry(stat);

            this.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10 + 50 * this.statList.Count);
            this.scrollbar.value = 1;
        }

        private void CreateEntry(KeyValuePair<string, float> stat)
        {
            var newPrefab = Instantiate(this.statPrefab, this.transform);
            
            var (nameText, valueText) = GetTextComponents(newPrefab);
            nameText.text = stat.Key;
            var value = Math.Round(stat.Value, 2);
            valueText.text = value.ToString(CultureInfo.InvariantCulture);
            
            this.statList.Add(newPrefab);
        }

        private void CreateSubtopic(string subtopicName)
        {
            var newPrefab = Instantiate(this.subtopicPrefab, this.transform);
            
            var textComponents = newPrefab.GetComponentsInChildren<TextMeshProUGUI>();
            var nameText = textComponents.First(c => c.gameObject.name.Equals("Name"));
            nameText.text = subtopicName;
            
            this.statList.Add(newPrefab);
        }

        private static (TextMeshProUGUI name, TextMeshProUGUI value) GetTextComponents(GameObject statObject)
        {
            var textComponents = statObject.GetComponentsInChildren<TextMeshProUGUI>(); 
            var nameText = textComponents.First(c => c.gameObject.name.Equals("Name"));
            var valueText = textComponents.First(c => c.gameObject.name.Equals("Value"));

            return (nameText, valueText);
        }

        private static long CalculateHighscore()
        {
            var levels = StatCollector.GeneralStats.FirstOrDefault(pair => pair.Key.Equals("Levels Completed")).Value;
            var kills = StatCollector.GeneralStats.FirstOrDefault(pair => pair.Key.Equals("Enemies Killed")).Value;
            var damageTaken = StatCollector.GeneralStats.FirstOrDefault(pair => pair.Key.Equals("Damage Taken")).Value;

            var causedDamage = 0f;
            foreach (var pair in StatCollector.WeaponStats)
            {
                causedDamage += pair.Value;
            }

            return (long) (levels * 100 + kills - damageTaken + causedDamage);
        }
    }
}