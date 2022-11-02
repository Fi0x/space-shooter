using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
    public class GameText : MonoBehaviour
    {
        [SerializeField] private Slider bar;
        public string CustomId { get; set; }
        public bool PermanentText { get; set; }
        private float barPercentage;

        public float BarPercentage
        {
            get => this.barPercentage;
            set
            {
                if (this.bar == null)
                    throw new NullReferenceException("Object does not have a bar to display a percentage");
                this.barPercentage = value;
                this.bar.value = this.barPercentage;
            }
        }
    }
}
