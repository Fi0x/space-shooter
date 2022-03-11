using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        public Sound[] musicClips;
        [SerializeField] private AudioMixer mixer;

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
                s.source.outputAudioMixerGroup = this.mixer.FindMatchingGroups("Music").First();
            }
        }

        private void Start()
        {
            this.Play("Ambience");
        }

        private void Play(string soundName)
        {
            var s = Array.Find(this.musicClips, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }
            s.source.Play();
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

        [Range(0f, 1f)] 
        public float volume;
        [Range(0.1f, 3f)] 
        public float pitch;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}
