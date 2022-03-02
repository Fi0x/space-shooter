using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSound : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    public AudioClip[] clips;
    public Vector2 pitchRange = new Vector2(1f,1f);
    public Vector2 volumeRange = new Vector2(.1f, .1f);
    
    // Start is called before the first frame update
    void Start()
    {
        int i = Random.Range(0, clips.Length - 1);
        source.clip = clips[i];
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.volume = Random.Range(volumeRange.x, volumeRange.y);
        source.Play();
    }
}
