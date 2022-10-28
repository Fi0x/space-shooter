#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Ui3D
{
    public class Ui3DManager : MonoBehaviour
    {
        [SerializeField] private Transform uiRoot = null!;
        [SerializeField] private float distanceToRoot = 5;

        private List<Ui3DElement> uiElements = new List<Ui3DElement>();

        public Transform UiRoot => this.uiRoot;
        public float DistanceToRoot => this.distanceToRoot;

        private void Start()
        {
            if (this.uiRoot == null)
            {
                throw new NullReferenceException("Ui Root is not set");
            }
        }

        private void LateUpdate()
        {
            foreach (var entry in this.uiElements)
            {
                if (entry != null)
                {
                    entry.UpdateElement(this);
                }
            }
        }

        public void NotifyAboutNewElement(Ui3DElement element)
        {
            this.uiElements.Add(element);
        }

        public void RemoveElement(Ui3DElement element)
        {
            this.uiElements.Remove(element);
        }
    }
}