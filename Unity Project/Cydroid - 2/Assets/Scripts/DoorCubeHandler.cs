using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorCubeHandler : MonoBehaviour {

    public bool activated = true;
    public Animator animator;
    [Range(0, 7)] public byte cubeColor = 0;
    public int cubeNeeded = 1;
    public bool atLeast = false;
    public int cubesIn = 0;

    [ReadOnly] public bool opened;

    void CheckForOpening() {
        if (atLeast ? cubesIn >= cubeNeeded : cubesIn == cubeNeeded) {
            opened = true;
            if(animator != null) {
                if (animator.GetBool("Opened") == false) {
                    animator.SetBool("Opened", true);
                }
            }
        } else {
            opened = false;
            if(animator != null) {
                if (animator.GetBool("Opened") == true) {
                    animator.SetBool("Opened", false);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        CheckForOpening();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerEnter(Collider other) {
        if(activated && other.TryGetComponent(out Cube cube)) {
            if (cubeColor == 0 || cube.cubeColor == cubeColor) {
                cubesIn += cube.cubeSize;
                CheckForOpening();
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if(activated && other.TryGetComponent(out Cube cube)) {
            if (cubeColor == 0 || cube.cubeColor == cubeColor) {
                cubesIn -= cube.cubeSize;
                CheckForOpening();
            }
        }
    }
}
