using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    [System.Serializable]
    public class NamedAudioClip
    {
        public string name;
        public AudioClip clip;
    }

    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField]
        private List<NamedAudioClip> audioClips = new();

        private Dictionary<string, AudioClip> clipLookup;

        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            clipLookup = new Dictionary<string, AudioClip>();

            foreach (var namedClip in audioClips)
            {
                if (!clipLookup.ContainsKey(namedClip.name) && namedClip.clip != null)
                {
                    clipLookup.Add(namedClip.name, namedClip.clip);
                }
            }
        }

        public void Play(string name)
        {
            if (clipLookup.TryGetValue(name, out AudioClip clip))
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }

}