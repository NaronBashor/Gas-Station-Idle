using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private int initialAudioSourceCount = 5;

    private Dictionary<string, AudioClip> clipDictionary;
    private List<AudioSource> audioSources;

    private void Awake()
    {
        Instance = this;

        clipDictionary = new Dictionary<string, AudioClip>{
            { "vehicle", audioClips[0] },
            { "buttonClick", audioClips[1] },
            { "carFill", audioClips[2] },
            { "moneyIcon", audioClips[3] }
        };

        // Initialize a pool of AudioSources
        audioSources = new List<AudioSource>();
        for (int i = 0; i < initialAudioSourceCount; i++) {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        audioSources.Add(newSource);
        return newSource;
    }

    public void PlaySound(string soundName, float volume, bool shouldLoop = false)
    {
        if (clipDictionary.TryGetValue(soundName, out AudioClip clip)) {
            // Find an available AudioSource or create a new one if necessary
            AudioSource availableSource = audioSources.Find(source => !source.isPlaying) ?? CreateNewAudioSource();

            // Set the clip, volume, and loop
            availableSource.clip = clip;
            availableSource.loop = shouldLoop;
            availableSource.volume = volume;

            // Play the clip
            availableSource.Play();
        } else {
            Debug.LogWarning("Sound not found: " + soundName);
        }
    }
}
