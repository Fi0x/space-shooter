using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PauseVfx : MonoBehaviour
{
    public VisualEffect vfx;
    public float delay;

    private void Start()
    {
        StartCoroutine(PauseOverSec(delay));
    }

    private IEnumerator PauseOverSec(float value)
    {
        float elapsed = 0f;

        while (elapsed < value)
        {
            elapsed += Time.unscaledDeltaTime;
            vfx.playRate = Mathf.Lerp(1f, 0.01f, elapsed / value);
            yield return null;
        }

        vfx.pause = true;
    }
}
