using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class Narration : MonoBehaviour {

    [Serializable]
    public struct SubtitleItem {
        [Tooltip("In millisec.")] [Range(0, 65535)] public ushort delay;
        public Subtitle subtitle;
    }

    public List<SubtitleItem> subtitles;
    public bool finishedOnce;
    public bool canBeSkipped;
    public bool canBeReplayed;
    public UnityEvent onStart;
    public UnityEvent onStop;
    public UnityEvent onFirstStart;
    public UnityEvent onFirstStop;
    [Tooltip("Objects to (de)activate on start/finish.")] public List<GameObject> linkedObjects;

    [Space(10)]

    public bool isPlaying = false;
    public int currentSubtitle = -1;

    void Start() {
        subtitles = new List<SubtitleItem>();
        foreach (Subtitle subtitle in GetComponentsInChildren<Subtitle>()) {
            subtitles.Add(new SubtitleItem() {
                delay = 0,
                subtitle = subtitle
            });
        }
    }

    void Update() {
        
    }

    public void Play() {
        isPlaying = true;
        Debug.Log("******************************** Playing narration " + gameObject.name);
        currentSubtitle = 0;
        if (linkedObjects != null) {
            linkedObjects.ForEach((obj) => obj.SetActive(false));
        }
        if (!finishedOnce) {
            onFirstStart.Invoke();
        }
        onStart.Invoke();
        StartCoroutine(DoAllTheThings());
    }

    public void Stop() {
        if (!finishedOnce) {
            onFirstStop.Invoke();
        }
        onStop.Invoke();
        isPlaying = false;
        finishedOnce = true;
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
                Debug.Log("******************************** Playing sub ");
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
