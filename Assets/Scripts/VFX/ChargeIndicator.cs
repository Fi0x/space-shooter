using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ChargeIndicator : MonoBehaviour
{
    public VisualEffect effect;

    [SerializeField] private Color chargingColor = Color.yellow;
    [SerializeField] private Color chargedColor = Color.green;
    
    private int colorId;
    private int timeId;
    private void Start()
    {
        colorId = Shader.PropertyToID("Color");
        timeId = Shader.PropertyToID("Time");
        this.Charge(1);
    }

    public void Charge(float time)
    {
        StartCoroutine(this.ChargeCoroutine(time));
    }

    private IEnumerator ChargeCoroutine(float time)
    {
        effect.Stop();
        SetColor(this.chargingColor);
        effect.SetFloat(timeId, time);
        effect.Play();
        yield return new WaitForSeconds(time);
        SetColor(this.chargedColor);
    }

    private void SetColor(Color color)
    {
        effect.SetVector4(colorId, color);
        
    }
}
