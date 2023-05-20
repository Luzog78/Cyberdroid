using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour {
    public static Manager instance;
    public static int currentLevel = 0;

    public static void ResetGame() {
        currentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        currentLevel++;
    }

    public static void Quit() {
        Debug.Log("Quit");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    [Serializable]
    public struct Color {
        public string name;
        public Vector3 color;
        public Material block;
        public Material fx;
        public Material transparent;
    }

    [Serializable]
    public struct TimeSpent {
        public int room;
        public float duration;
    }

    [Serializable]
    public struct NarrationItem {
        public string id;
        public Narration narration;
    }

    [Serializable]
    public struct UIControl {
        public TextMeshProUGUI textBox;
        public List<GameObject> otherObjects;

        public UIControl(TextMeshProUGUI textBox, List<GameObject> otherObjects) {
            this.textBox = textBox;
            this.otherObjects = otherObjects;
        }
    }

    public bool isPaused = false;
    private bool __isPaused;

    [Header("Gameplay")]
    [Space(10)]
    public List<PlayerHandler> players = new List<PlayerHandler>();
    public int deaths = 0;
    public int room = -1;
    public List<TimeSpent> timeSpent = new List<TimeSpent>();
    
    [Header("Utils")]
    [Space(10)]
    public Material glass;
    public PhysicMaterial noFriction;
    public List<Color> colors = new List<Color>();
    
    [Header("UI")]
    [Space(10)]
    public GameObject pauseUI          = null;
    public UIControl uiControlJump     = new UIControl(null, new List<GameObject>());
    public UIControl uiControlRun      = new UIControl(null, new List<GameObject>());
    public UIControl uiControlForward  = new UIControl(null, new List<GameObject>());
    public UIControl uiControlBackward = new UIControl(null, new List<GameObject>());
    public UIControl uiControlLeft     = new UIControl(null, new List<GameObject>());
    public UIControl uiControlRight    = new UIControl(null, new List<GameObject>());
    public UIControl uiControlInteract = new UIControl(null, new List<GameObject>());
    public UIControl uiControlReset    = new UIControl(null, new List<GameObject>());
    public UIControl uiControlExit     = new UIControl(null, new List<GameObject>());
    public UIControl uiControlRoom     = new UIControl(null, new List<GameObject>());
    public UIControl uiControlDeaths   = new UIControl(null, new List<GameObject>());

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
    public List<NarrationItem> voices = new List<NarrationItem>();
    public List<Narration> currentVoice = new List<Narration>();

    public struct RoomHandler {
        public int room;
        public System.Action onEnterTheFirstTime;
        public System.Action onEnter;
        public System.Action onExit;
        public Dictionary<float, System.Action> onTimeSpent;

        public RoomHandler(int room, System.Action onEnterTheFirstTime, System.Action onEnter, System.Action onExit,
                           Dictionary<float, System.Action> onTimeSpent) {
            this.room = room;
            this.onEnterTheFirstTime = onEnterTheFirstTime;
            this.onEnter = onEnter;
            this.onExit = onExit;
            this.onTimeSpent = onTimeSpent;
        }

        public void OnEnterTheFirstTime() {
            onEnterTheFirstTime?.Invoke();
        }

        public void OnEnter() {
            onEnter?.Invoke();
        }

        public void OnExit() {
            onExit?.Invoke();
        }

        public void OnTimeSpent(float from, float to) {
            if (onTimeSpent != null) {
                foreach (KeyValuePair<float, System.Action> pair in onTimeSpent) {
                    if (pair.Key >= from && pair.Key < to) {
                        pair.Value?.Invoke();
                    }
                }
            }
        }
    }

    public List<RoomHandler> rooms = new List<RoomHandler>();

    public RoomHandler GetRoomHandler(int room) {
        foreach (RoomHandler roomHandler in rooms) {
            if (roomHandler.room == room) {
                return roomHandler;
            }
        }
        return new RoomHandler(-1, null, null, null, null);
    }

    private float UpdateVolumeObject(GameObject volumeObject) {
        Slider slider = volumeObject.GetComponentInChildren<Slider>();
        TextMeshProUGUI text = volumeObject.GetComponentsInChildren<TextMeshProUGUI>()[1];
        var i = Mathf.RoundToInt(slider.value * 1000) / 10f;
        if (i == (int) i)
            i = (int) i;
        text.text = (i + "").Replace(",", ".");
        return slider.value;
    }

    public float GetBackgroundVolume() {
        return backgroundVolume;
    }

    public void SetBackgroundVolume(float volume) {
        backgroundVolume = volume;
    }

    public void SetBackgroundVolume(GameObject volumeObject) {
        backgroundVolume = UpdateVolumeObject(volumeObject);
    }

    public float GetSfxVolume() {
        return sfxVolume;
    }

    public void SetSfxVolume(float volume) {
        sfxVolume = volume;
    }

    public void SetSfxVolume(GameObject volumeObject) {
        sfxVolume = UpdateVolumeObject(volumeObject);
    }

    public float GetVoiceVolume() {
        return voiceVolume;
    }

    public void SetVoiceVolume(float volume) {
        voiceVolume = volume;
    }

    public void SetVoiceVolume(GameObject volumeObject) {
        voiceVolume = UpdateVolumeObject(volumeObject);
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

    void Awake() {
        if (FindObjectsOfType<Manager>().Length > 1) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start() {
        __isPaused = !isPaused;

        Dictionary<float, System.Action> onTimeSpent = new Dictionary<float, System.Action>();
        rooms.Add(new RoomHandler(0, () => {

        }, () => {

        }, () => {

        }, onTimeSpent));
        rooms.Add(new RoomHandler(1, () => {

        }, () => {

        }, () => {

        }, onTimeSpent));
        rooms.Add(new RoomHandler(2, () => {

        }, () => {

        }, () => {

        }, onTimeSpent));
    }

    // Update is called once per frame
    void Update() {
        instance = this;

        if (isPaused != __isPaused) {
            __isPaused = isPaused;
            if (isPaused) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (pauseUI != null)
                pauseUI.SetActive(isPaused);
        }
        
        if (deaths == -1) {
            if (uiControlDeaths.textBox != null)
                uiControlDeaths.textBox.text = "∞";
            uiControlDeaths.otherObjects.ForEach(go => go.SetActive(false));
        } else {
            if (uiControlDeaths.textBox != null)
                uiControlDeaths.textBox.text = deaths.ToString();
            uiControlDeaths.otherObjects.ForEach(go => go.SetActive(true));
        }
        if (room == -1) {
            if (uiControlRoom.textBox != null)
                uiControlRoom.textBox.text = "∞";
            uiControlRoom.otherObjects.ForEach(go => go.SetActive(false));
        } else {
            if (uiControlRoom.textBox != null)
                uiControlRoom.textBox.text = "S" + (-room);
            uiControlRoom.otherObjects.ForEach(go => go.SetActive(true));
        }

        ClearSounds(currentBackgrounds);
        ClearSounds(currentSfx);

        if (currentBackgrounds.Count == 0) {
            int random = UnityEngine.Random.Range(0, backgrounds.Count);
            PlayBackground(random);
        }

        bool found = false;
        RoomHandler rh = GetRoomHandler(room);
        for (int i = 0; i < timeSpent.Count; i++) {
            if (timeSpent[i].room == room) {
                float from = timeSpent[i].duration;
                timeSpent[i] = new TimeSpent() {
                    room = room,
                    duration = timeSpent[i].duration + Time.deltaTime
                };
                found = true;
                if (rh.room != -1) {
                    rh.OnTimeSpent(from, timeSpent[i].duration);
                }
                break;
            }
        }
        if (!found) {
            timeSpent.Add(new TimeSpent() {
                room = room,
                duration = 1 * Time.deltaTime
            });
            if (rh.room != -1) {
                rh.onEnterTheFirstTime();
            }
        }
    }
}