using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorSlideHandler : MonoBehaviour
{

    public bool activated = true;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(animator != null)
        {
            animator.SetBool("Opened", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(animator != null)
        {
            animator.SetBool("Opened", false);
        }
    }
}
