using UnityEngine;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour {
    public static Color baseColor = new Color(1f, 0f, 0f, 0.376f);
    public static Color checkedColor = new Color(0f, 1f, 0f, 0.376f);

    public bool isChecked = false;
    public Image image;

    public void Toggle() {
        isChecked = !isChecked;
    }

    public void Check() {
        isChecked = true;
    }

    public void Uncheck() {
        isChecked = false;
    }

    // Start is called before the first frame update
    void Start() {
        if (image == null)
            image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        if (image == null)
            return;
        image.color = isChecked ? checkedColor : baseColor;
    }
}
