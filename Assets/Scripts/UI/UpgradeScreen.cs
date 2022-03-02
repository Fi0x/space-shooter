using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UpgradeScreen : MonoBehaviour
    {
        [SerializeField] private GameObject upgradePrefab;
        [SerializeField] private Scrollbar scrollbar;

        private void Start()
        {
            
            
            this.scrollbar.value = 1;
        }
    }
}