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

    public class AudioContoller : MonoBehaviour
    {
        public static AudioContoller Instance { get; private set; }

        [SerializeField]
        private List<NamedAudioClip> audioClips = new();

        private Dictionary<string, AudioClip> clipLookup;

        private AudioSource audioSource;

        private AudioClip currentClip;
        private float currentClipStartTime;

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
            if (!clipLookup.TryGetValue(name, out AudioClip newClip))
                return;

            if (audioSource.isPlaying && currentClip != null)
            {
                float elapsed = Time.time - currentClipStartTime;

                if (elapsed < currentClip.length * 0.5f)
                {
                    return;
                }
                audioSource.Stop();
            }

            audioSource.clip = newClip;
            audioSource.Play();
            currentClip = newClip;
            currentClipStartTime = Time.time;
        }
    }

}