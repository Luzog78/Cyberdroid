using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Subtitle : MonoBehaviour {
    
    [Serializable]
    public struct Sentence {
        [TextArea(5, 10)] public string text;
        [Tooltip("In millisec.")] [Range(0, 65535)] public ushort duration;
    }

    [Range(-128, 127)] public sbyte priority;
    public AudioSource audioSource;
    public TextMeshProUGUI textBox;
    public List<GameObject> linkedObjects;
    public bool isPlaying = false;
    public int currentSentence = -1;
    public List<Sentence> text;

    // Start is called before the first frame update
    void Start() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (textBox == null)
            textBox = GetComponent<TextMeshProUGUI>();
        Play();
    }

    // Update is called once per frame
    void Update() {
        if (isPlaying) {
            if (currentSentence == -1 && (audioSource == null || !audioSource.isPlaying)) {
                Stop();
            }
        }
    }

    public void Play() {
        isPlaying = true;
        if (audioSource != null) {
            audioSource.Play();
        }
        currentSentence = 0;
        if (textBox != null) {
            textBox.gameObject.SetActive(true);
            linkedObjects.ForEach((obj) => obj.SetActive(true));
        }
        StartCoroutine(ShowSentences());
    }

    public void Stop() {
        isPlaying = false;
        currentSentence = -1;
        if (audioSource != null) {
            audioSource.Stop();
        }
        if (textBox != null) {
            textBox.text = "";
            textBox.gameObject.SetActive(false);
            linkedObjects.ForEach((obj) => obj.SetActive(false));
        }
    }

    public IEnumerator ShowSentences() {
        if (!isPlaying) {
            yield break;
        }
        if (currentSentence >= text.Count) {
            currentSentence = -1;
            if (textBox != null) {
                textBox.text = "";
                textBox.gameObject.SetActive(false);
                linkedObjects.ForEach((obj) => obj.SetActive(false));
            }
            yield break;
        }
        if (textBox != null) {
            textBox.text = "<color=#dd0000><b><u>Narrateur :</u></b></color> <color=#fff>"
                            + text[currentSentence].text + "</color>";
        }
        yield return new WaitForSeconds(text[currentSentence].duration / 1000f);
        currentSentence++;
        StartCoroutine(ShowSentences());
    }
}
