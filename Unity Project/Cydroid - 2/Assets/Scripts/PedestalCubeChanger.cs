using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PedestalCubeChanger : MonoBehaviour {

    private Material originalMaterial;

    public bool activated = true;
    public MeshRenderer meshRenderer;
    public Material outMaterial;
    [Range(0, 7)] public short cubeColor = 0;
    public int cubeNeeded = 1;
    public bool atLeast = false;
    public int cubesIn = 0;

    void CheckForChangingMat() {
        if (atLeast ? cubesIn >= cubeNeeded : cubesIn == cubeNeeded) {
            meshRenderer.materials = new Material[] { outMaterial };
        } else {
            meshRenderer.materials = new Material[] { originalMaterial };
        }
    }

    // Start is called before the first frame update
    void Start() {
        originalMaterial = meshRenderer.materials.Count() > 0 ? meshRenderer.materials[0] : null;
        CheckForChangingMat();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerEnter(Collider other) {
        if(activated && other.TryGetComponent(out Cube cube)) {
            if (cubeColor == 0 || cube.cubeColor == cubeColor) {
                cubesIn += cube.cubeSize;
                CheckForChangingMat();
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if(activated && other.TryGetComponent(out Cube cube)) {
            if (cubeColor == 0 || cube.cubeColor == cubeColor) {
                cubesIn -= cube.cubeSize;
                CheckForChangingMat();
            }
        }
    }
}
