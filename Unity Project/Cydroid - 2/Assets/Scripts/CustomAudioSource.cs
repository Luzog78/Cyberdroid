using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAudioSource : MonoBehaviour {

    public AudioSource audioSource;
    private float volume;
    private System.Func<float> getVolume;

    [Header("Read Only")]
    [Space(10)]
    public bool isPlaying = false;
    [Range(0f, 1f)] public float originalVolume;
    [Range(0f, 1f)] public float progress = 0f;

    // Start is called before the first frame update
    void Start() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        volume = audioSource.volume;
    }

    // Update is called once per frame
    void Update() {
        isPlaying = IsPlaying();
        originalVolume = volume;
        progress = audioSource.time / audioSource.clip.length;
        if (isPlaying) {
            audioSource.volume = volume * (getVolume == null ? 1f : getVolume());
        }
    }

    public void Play(System.Func<float> getVolume) {
        this.getVolume = getVolume;
        audioSource.Play();
    }

    public void Stop() {
        audioSource.Stop();
        audioSource.volume = volume;
    }

    public void Pause() {
        audioSource.Pause();
    }

    public void UnPause() {
        audioSource.UnPause();
    }
    
    public bool IsPlaying() {
        return audioSource.isPlaying;
    }

    public bool IsPaused() {
        return !audioSource.isPlaying && audioSource.time > 0f;
    }

    public float GetVolume() {
        return volume;
    }

    public void SetVolume(float volume) {
        this.volume = volume;
        audioSource.volume = volume;
    }
}
