using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ChargeIndicator : MonoBehaviour
{
    public VisualEffect effect;

    private int colorId;
    private int timeId;
    private void Start()
    {
        colorId = Shader.PropertyToID("Color");
        timeId = Shader.PropertyToID("Time");
    }

    public void Charge(float time)
    {
        //effect.Stop();
        effect.SetFloat(timeId, time);
        effect.Play();
    }

    public void SetColor(Color color)
    {
        effect.SetVector4(colorId, color);
        
    }
}
