using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Stats;
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
            
            var textComponents = newPrefab.GetComponentsInChildren<Text>();
            var nameText = textComponents.First(c => c.gameObject.name.Equals("Name"));
            nameText.text = subtopicName;
            
            this.statList.Add(newPrefab);
        }

        private static (Text name, Text value) GetTextComponents(GameObject statObject)
        {
            var textComponents = statObject.GetComponentsInChildren<Text>(); 
            var nameText = textComponents.First(c => c.gameObject.name.Equals("Name"));
            var valueText = textComponents.First(c => c.gameObject.name.Equals("Value"));

            return (nameText, valueText);
        }
    }
}