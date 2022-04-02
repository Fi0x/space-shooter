using System;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UiSound : MonoBehaviour, IPointerEnterHandler
    {
        public string enterSound = "GUIEnter";

        public void Play(string soundName)
        {
            AudioManager.instance.Play(soundName);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("MouseEnter");
            AudioManager.instance.Play(enterSound);
        }
    }
}