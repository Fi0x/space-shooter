using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RocketTrailPause : MonoBehaviour
{
    public CinemachineDollyCart cart;
    public ParticleSystem particle;
    public float time = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FollowPath(time));
    }
    
    private IEnumerator FollowPath(float secs)
    {
        float elapsed = 0f;

        while (elapsed < secs)
        {
            elapsed += Time.unscaledDeltaTime;
            cart.m_Position = Mathf.Lerp(0f, 1f, elapsed / secs);
            yield return null;
        }

        //cart.m_Position = 1f;
        particle.Pause();
    }
}
