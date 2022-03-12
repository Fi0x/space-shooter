using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ship.Weaponry;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatScreen : MonoBehaviour
    {
        [SerializeField] private GameObject statPrefab;
        [SerializeField] private Scrollbar scrollbar;
        
        private readonly List<GameObject> statList = new List<GameObject>();

        private void OnEnable()
        {
            this.UpdateStatList();
        }

        private void UpdateStatList()
        {
            this.statList.Clear();

            foreach (var stat in StatCollector.Stats)
            {
                var newPrefab = Instantiate(this.statPrefab, this.transform);
                
                var (nameText, valueText) = GetTextComponents(newPrefab);
                nameText.text = stat.Key;
                var value = Math.Round((double) stat.Value, 2);
                valueText.text = value.ToString(CultureInfo.InvariantCulture);
                
                this.statList.Add(newPrefab);
            }

            this.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10 + 50 * this.statList.Count);
            this.scrollbar.value = 1;
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