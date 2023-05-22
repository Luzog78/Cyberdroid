using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject[] objectsToActivate;
    public TextMeshProUGUI text;
    private string originalText;
    [TextArea(5, 10)] public string hoverText;

    void Start()
    {
        originalText = text.text;
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        Debug.Log("Cursor Entering " + name + " GameObject");
        text.text = hoverText;
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        Debug.Log("Cursor Exiting " + name + " GameObject");
        text.text = originalText;
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(false);
        }
    }
}