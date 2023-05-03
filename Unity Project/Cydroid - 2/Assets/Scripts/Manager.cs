using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {
    public static Manager instance;
    public static int currentLevel = 0;

    public static void ResetGame() {
        currentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        currentLevel++;
    }
    
    [Header("Colors")]
    [Space(10)]
    public List<String> names = new List<String>();
    public List<Material> blocks = new List<Material>();
    public List<Material> fx = new List<Material>();
    public List<Material> transparent = new List<Material>();
    
    [Header("Utils")]
    [Space(10)]
    public Material glass;
    public PhysicMaterial noFriction;

    [Header("Sounds")]
    [Space(10)]
    [Range(0, 1)] public float backgroundVolume = 1f;
    public List<AudioSource> backgrounds = new List<AudioSource>();
    public List<CustomAudioSource> currentBackgrounds = new List<CustomAudioSource>();
    [Space(10)]
    [Range(0, 1)] public float sfxVolume = 1f;
    public List<AudioSource> sfx = new List<AudioSource>();
    public List<CustomAudioSource> currentSfx = new List<CustomAudioSource>();
    [Space(10)]
    [Range(0, 1)] public float voiceVolume = 1f;
    public List<AudioSource> voices = new List<AudioSource>();
    public List<CustomAudioSource> currentVoice = new List<CustomAudioSource>();

    public float GetBackgroundVolume() {
        return backgroundVolume;
    }

    public float GetSfxVolume() {
        return sfxVolume;
    }

    public float GetVoiceVolume() {
        return voiceVolume;
    }

    public int ClearSounds(List<CustomAudioSource> currentList) {
        int count = 0;
        foreach (CustomAudioSource cas in new List<CustomAudioSource>(currentList)) {
            if (!cas.IsPlaying() && !cas.IsPaused()) {
                cas.Stop();
                currentList.Remove(cas);
                Destroy(cas.gameObject);
                count++;
            }
        }
        return count;
    }

    public int PlaySound(List<AudioSource> list, List<CustomAudioSource> currentList,
                         System.Func<float> getVolume, int index, bool stopOthers = false) {
        if (index >= 0 && index < list.Count) {
            if (stopOthers && currentList.Count > 0) {
                foreach (CustomAudioSource audioSource in currentList) {
                    audioSource.Stop();
                }
                currentList.Clear();
            }
            GameObject go = Instantiate(list[index].gameObject, transform);
            CustomAudioSource cas = go.GetOrAddComponent<CustomAudioSource>();
            cas.audioSource = go.GetComponent<AudioSource>();
            currentList.Add(cas);
            cas.Play(GetBackgroundVolume);
            return currentList.Count - 1;
        }
        return -1;
    }

    public int PlayBackground(int index, bool stopOthers = false) {
        return PlaySound(backgrounds, currentBackgrounds, GetBackgroundVolume, index, stopOthers);
    }

    public int PlaySfx(int index, bool stopOthers = false) {
        return PlaySound(sfx, currentSfx, GetSfxVolume, index, stopOthers);
    }

    public int PlayVoice(int index, bool stopOthers = false) {
        return PlaySound(voices, currentVoice, GetVoiceVolume, index, stopOthers);
    }

    void Awake() {
        if (FindObjectsOfType<Manager>().Length > 1) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        int i = ClearSounds(currentBackgrounds);
        ClearSounds(currentSfx);
        ClearSounds(currentVoice);

        if (currentBackgrounds.Count == 0) {
            int random = UnityEngine.Random.Range(0, backgrounds.Count);
            PlayBackground(random);
        }
    }
}