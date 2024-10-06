using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance { get; private set; }

    public SoundClip[] soundClips; // Array of sound clips with individual settings
    private AudioSource myAudioSource;
    void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate SoundManagers
        }
    }

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();

      
    }

    // Method to play a specific sound by name
    public void PlaySound(string soundName, AudioSource audioSource)
    {
        SoundClip selectedClip = GetSoundClipByName(soundName);
        if (selectedClip != null)
        {
            ApplySoundSettings(selectedClip, audioSource);
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Sound with name {soundName} not found!");
        }
    }


    public void PlaySound(string soundName)
    {
        SoundClip selectedClip = GetSoundClipByName(soundName);
        if (selectedClip != null)
        {
            ApplySoundSettings(selectedClip, myAudioSource);
            myAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Sound with name {soundName} not found!");
        }
    }

    public void PlayButton()
    {
        PlaySound("BTN");
    }

    public void CancelButton()
    {
        PlaySound("BTN_Cancel");
    }

    // Helper method to find a sound clip by name
    private SoundClip GetSoundClipByName(string soundName)
    {
        foreach (SoundClip soundClip in soundClips)
        {
            if (soundClip.name == soundName)
            {
                return soundClip;
            }
        }
        return null; // Return null if sound not found
    }

    // Applies volume and pitch settings to the provided AudioSource
    private void ApplySoundSettings(SoundClip soundClip, AudioSource audioSource)
    {
        audioSource.clip = soundClip.clip; // Set the clip
        audioSource.volume = soundClip.volume; // Set the volume specific to this clip
        audioSource.pitch = soundClip.pitch;   // Set the pitch specific to this clip
    }
}


[System.Serializable]
public class SoundClip
{
    public string name; // Name to identify the sound
    public AudioClip clip; // The audio clip
    [Range(0f, 1f)] public float volume = 1f; // Volume for this specific clip
    [Range(0.1f, 3f)] public float pitch = 1f; // Pitch for this specific clip
}