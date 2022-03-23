using System;
using System.Collections.Generic;
using Ship.Rocket;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RocketIndicator : MonoBehaviour
    {
        [Header("Dependencies")]
        public RocketSpawner spawner;
        public GameObject prefab;
        public List<Image> images = new List<Image>();

        [Header("Colors")]
        public Color activeColor = Color.cyan;
        public Color chargingColor = Color.grey;
        public Color inactiveColor = Color.black;

        private void Update()
        {
            CheckMaxRockets();
            UpdateColors();
        }

        private void UpdateColors()
        {
            for (int i = 0; i < images.Count; i++)
            {
                var k = images.Count - i - 1;
                if (i < spawner.currentCharges)
                {
                    images[k].color = activeColor;
                    images[k].fillAmount = 1f;
                }
                else if(i < spawner.currentCharges + 1)
                {
                    images[k].color = chargingColor;
                    images[k].fillAmount = spawner.chargePct;
                }
                else
                {
                    images[k].color = inactiveColor;
                    images[k].fillAmount = 1f;
                }
            }
        }

        private void CheckMaxRockets()
        {
            if (spawner.maxRocketCharges > images.Count)
            {
                for (int i = 0; i < spawner.maxRocketCharges - images.Count; i++)
                {
                    var obj = Instantiate(prefab, transform);
                    images.Add(obj.GetComponent<Image>());
                }
            }else
            if (spawner.maxRocketCharges < images.Count)
            {
                for (int i = 0; i < images.Count - spawner.maxRocketCharges; i++)
                {
                    Destroy(images[images.Count - 1]);
                    images.RemoveAt(images.Count - 1);
                }
            }
        }
    }
}
