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
        this.slider.maxValue = maxHealth;
        this.slider.value = maxHealth;

        this.healthNumber_TMP.text = maxHealth + " / " + maxHealth;
    }

    public void SetCurrentHealth(int health)
    {
        this.slider.value = health;

        this.healthNumber_TMP.text = this.slider.value + " / " + this.slider.maxValue;
    }
}
