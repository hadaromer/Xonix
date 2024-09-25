using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource _audioSource; // AudioSource component to play sounds
    [SerializeField] private List<AudioClip> _audioClips; // List of audio clips

    // Property to indicate if music is allowed
    public bool IsSoundEffectAllowed { get; set; } = true;

    // Property to indicate if looping music is allowed
    public bool IsMusicLoopAllowed { get; set; } = true;

    // Play a one-shot sound by clip name
    public void PlaySound(string clipName)
    {
        if (!IsSoundEffectAllowed) return;

        AudioClip clip = _audioClips.Find(c => c.name == clipName);
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Audio clip {clipName} not found!");
        }
    }

    // Play a sound in an infinite loop by clip name
    public void PlaySoundLoop(string clipName)
    {
        if (!IsMusicLoopAllowed) return;

        AudioClip clip = _audioClips.Find(c => c.name == clipName);
        if (clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.loop = true;
            _audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Audio clip {clipName} not found!");
        }
    }

    // Stop the currently playing looping sound
    public void StopSoundLoop()
    {
        _audioSource.loop = false;
        _audioSource.Stop();
    }
}