using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        public Sound[] musicClips;
        public AudioSource activeMusic = null;
        [SerializeField] public AudioMixer mixer;
        
        public static event EventHandler AudioManagerInitializedEvent;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }
        
            DontDestroyOnLoad(this.gameObject);

            foreach (var s in this.musicClips)
            {
                s.source = this.gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.outputAudioMixerGroup = s.mixerGroup;
            }

            AudioManagerInitializedEvent?.Invoke(null, null);
        }

        private void Start()
        {
            this.ChangeMusic("MainMusic");
        }

        public void Play(string soundName)
        {
            var s = Array.Find(instance.musicClips, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }
            s.source.Play();
        }

        public void ChangeMusic(string newMusic)
        {
            var s = Array.Find(instance.musicClips, sound => sound.name == newMusic);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + newMusic + " not found!");
                return;
            }
            if(activeMusic == s.source) return;
            if (activeMusic == null)
            {
                Debug.Log("no active music");
                s.source.Play();
                activeMusic = s.source;
                return;
            }
            StartCoroutine(FadeMusic(activeMusic, s.source, 2f));
        }

        IEnumerator FadeMusic(AudioSource from, AudioSource to, float time)
        {
            if (from == null) yield break;
            var oldFromVolume = from.volume;
            for (float t = 0f; t < time; t += Time.deltaTime)
            {
                from.volume = oldFromVolume * (1f - (t / time));
                yield return null;
            }
            from.Stop();
            
            to.Play();
            var oldToVolume = to.volume;
            for (float k = 0f; k < time; k += Time.deltaTime)
            {
                to.volume = oldToVolume * (k / time);
                yield return null;
            }
            from.volume = oldFromVolume;
            to.volume = oldToVolume;
            activeMusic = to;
        }

        public void UpdateMasterVolume(float volume)
        {
            this.mixer.SetFloat("masterVolume", volume);
        }
        public void UpdateMusicAmbientVolume(float volume)
        {
            this.mixer.SetFloat("musicVolume", volume);
        }
        public void UpdateEffectsVolume(float volume)
        {
            this.mixer.SetFloat("effectsVolume", volume);
        }
    }

    [Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;
        public AudioMixerGroup mixerGroup;

        [Range(0f, 1f)] 
        public float volume;
        [Range(0.1f, 3f)] 
        public float pitch;

        public bool loop;

        [HideInInspector, NonSerialized]
        public AudioSource source;
    }
}
