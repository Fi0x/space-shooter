using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

    [Header("Healthbar")]
    //
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private TextMeshProUGUI healthNumber_TMP;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;

        healthNumber_TMP.text = maxHealth + " / " + maxHealth;
    }

    public void SetCurrentHealth(int health)
    {
        slider.value = health;

        healthNumber_TMP.text = slider.value + " / " + slider.maxValue;
    }
}
