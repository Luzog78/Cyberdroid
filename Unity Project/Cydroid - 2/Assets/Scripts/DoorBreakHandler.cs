using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBreakHandler : MonoBehaviour
{

    public bool activated = true;
    public Breakable breakable;

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
        if(breakable != null && other.gameObject.tag == "Player")
        {
            breakable.broken = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        
    }
}
