using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ChargeIndicator : MonoBehaviour
{
    public List<VisualEffect> effects = new List<VisualEffect>();

    private int colorId;
    private int timeId;
    private void Start()
    {
        colorId = Shader.PropertyToID("Color");
        timeId = Shader.PropertyToID("Time");
    }

    public void Charge(float time)
    {
        foreach (var effect in effects)
        {
            //effect.Stop();
            effect.SetFloat(timeId, time);
            effect.Play();
        }
    }

    public void SetColor(Color color)
    {
        foreach (var effect in effects)
        {
            effect.SetVector4(colorId, color);
        }
    }
}
