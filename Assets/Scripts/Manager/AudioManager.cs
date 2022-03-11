using System;
using UnityEngine;

namespace Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        public Sound[] sounds;

        public static float EffectsVolume = 0.3f;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
            }
        
            DontDestroyOnLoad(gameObject);

            foreach (var s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        private void Start()
        {
            Play("Ambience");
        }

        public void Play(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }
            s.source.Play();
        }

        public void UpdateMusicAmbientVolume(float volume)
        {
            foreach (var s in this.sounds)
                s.source.volume = volume;
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
