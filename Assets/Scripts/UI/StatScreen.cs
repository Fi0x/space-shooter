using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatScreen : MonoBehaviour
    {
        [SerializeField] private GameObject statPrefab;
        [SerializeField] private Scrollbar scrollbar;
        
        private readonly List<GameObject> statList = new List<GameObject>();

        private void Start()
        {
            foreach (var stat in StatCollector.FloatStats)
            {
                var newPrefab = Instantiate(this.statPrefab, this.transform);
                
                var texts = GetTextComponents(newPrefab);
                texts.name.text = stat.Key;
                texts.value.text = stat.Value.ToString();
                
                this.statList.Add(newPrefab);
            }
            foreach (var stat in StatCollector.IntStats)
            {
                var newPrefab = Instantiate(this.statPrefab, this.transform);

                var texts = GetTextComponents(newPrefab);
                texts.name.text = stat.Key;
                texts.value.text = stat.Value.ToString();
                
                this.statList.Add(newPrefab);
            }

            this.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10 + 50 * this.statList.Count);
        }

        public void UpdateStats()
        {
            this.statList.ForEach(entry =>
            {
                var texts = GetTextComponents(entry);
                texts.value.text = StatCollector.GetValueStringForStat(texts.name.text);
            });
        }

        private static (Text name, Text value) GetTextComponents(GameObject statObject)
        {
            var textComponents = statObject.GetComponentsInChildren<Text>(); 
            var nameText = textComponents.Where(c => c.gameObject.name.Equals("Name"));
            var valueText = textComponents.Where(c => c.gameObject.name.Equals("Value"));

            return (nameText.First(), valueText.First());
        }
    }
}