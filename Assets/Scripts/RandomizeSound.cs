using System.Linq;
using Manager;
using UnityEngine;

public class RandomizeSound : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    public AudioClip[] clips;
    public Vector2 pitchRange = new Vector2(1f,1f);
    public Vector2 volumeRange = new Vector2(.1f, .1f);
    
    private void Start()
    {
        var i = Random.Range(0, this.clips.Length - 1);
        if (this.clips.Length == 0)
            i = 0;
        
        this.source.clip = this.clips[i];
        this.source.pitch = Random.Range(this.pitchRange.x, this.pitchRange.y);
        this.source.volume = Random.Range(this.volumeRange.x, this.volumeRange.y);
        this.source.outputAudioMixerGroup = AudioManager.Mixer.FindMatchingGroups("Effects").First();
        this.source.Play();
    }
}
