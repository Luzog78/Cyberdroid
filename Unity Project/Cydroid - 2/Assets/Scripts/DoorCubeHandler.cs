using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorCubeHandler : MonoBehaviour
{

    public bool activated = true;
    public Animator animator;
    public short cubeColor = 0;
    public int cubeNeeded = 1;
    public int cubesIn = 0;

    void CheckForOpening() {
        if (cubesIn >= cubeNeeded) {
            if(animator != null)
            {
                if (animator.GetBool("Opened") == false) {
                    animator.SetBool("Opened", true);
                }
            }
        } else {
            if(animator != null)
            {
                if (animator.GetBool("Opened") == true) {
                    animator.SetBool("Opened", false);
                }
            }
        }
    }

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
        if(activated && other.TryGetComponent(out Cube cube))
        {
            Debug.Log("Cube entered " + cube.cubeColor);
            if (cube.cubeColor == cubeColor) {
                cubesIn += cube.cubeSize;
                CheckForOpening();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(activated && other.TryGetComponent(out Cube cube))
        {
            Debug.Log("Cube exited " + cube.cubeColor);
            if (cube.cubeColor == cubeColor) {
                cubesIn -= cube.cubeSize;
                CheckForOpening();
            }
        }
    }
}
