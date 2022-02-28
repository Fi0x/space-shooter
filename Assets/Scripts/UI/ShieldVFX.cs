using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldVFX : MonoBehaviour
{

    [SerializeField] private Material shieldVFXMaterial;

    [SerializeField] private Color startColor;

    private void Start()
    {
        startColor = shieldVFXMaterial.color;
        startColor.a = 0.0f;

        shieldVFXMaterial.color = startColor;
    }

    public IEnumerator FadeIn()
    {
        float currentTime = 0f;
        float duration = 0.5f;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            Color oldColor = shieldVFXMaterial.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);

            shieldVFXMaterial.color = newColor;
            currentTime += Time.deltaTime;

            yield return null;
        }

        yield break;
    }

    /*
    public IEnumerator FadeOut()
    {

    }
    */
}
