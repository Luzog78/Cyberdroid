using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Narration : MonoBehaviour {

    [Serializable]
    public struct SubtitleItem {
        [Tooltip("In millisec.")] [Range(0, 65535)] public ushort delay;
        public Subtitle subtitle;
    }

    public bool activated;
    public List<SubtitleItem> subtitles;
    public bool finishedOnce;
    public bool canBeSkipped;
    public bool canBeReplayed;
    [Tooltip("Objects to (de)activate on start/finish.")] public List<GameObject> linkedObjects;

    [Space(10)]

    public bool isPlaying = false;
    public int currentSubtitle = -1;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Play() {
        isPlaying = true;
        currentSubtitle = 0;
        if (linkedObjects != null) {
            linkedObjects.ForEach((obj) => obj.SetActive(false));
        }
        StartCoroutine(DoAllTheThings());
    }

    public void Stop() {
        isPlaying = false;
        currentSubtitle = -1;
        if (linkedObjects != null) {
            linkedObjects.ForEach((obj) => obj.SetActive(true));
        }
        foreach (SubtitleItem item in subtitles) {
            item.subtitle.Stop();
        }
    }

    public IEnumerator DoAllTheThings() {
        if (!isPlaying || currentSubtitle >= subtitles.Count) {
            Stop();
            yield break;
        }
        if (currentSubtitle >= 0) {
            SubtitleItem item = subtitles[currentSubtitle];
            yield return new WaitForSeconds(item.delay / 1000f);
            if (item.subtitle != null) {
                item.subtitle.Play();
            }
            while (item.subtitle.isPlaying) {
                yield return null;
            }
            currentSubtitle++;
            StartCoroutine(DoAllTheThings());
        } else {
            currentSubtitle++;
            StartCoroutine(DoAllTheThings());
        }
    }
}
