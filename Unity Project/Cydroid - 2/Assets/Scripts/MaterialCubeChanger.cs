using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MaterialCubeChanger : MonoBehaviour {
    [Serializable]
    public enum Operator {
        AND, OR, XOR
    }

    [Serializable]
    public enum Comparaison {
        Equals, AtLeast, AtMost
    }

    [Serializable]
    public struct CubeInfo {
        [Range(0, 7)] public byte color;
        public int needed;
        public Comparaison comp;
        public int cubesIn;
    }

    private Material originalMaterial = null;

    public bool activated = true;
    public MeshRenderer meshRenderer;
    public Material outMaterial;
    public Operator operatorType = Operator.OR;
    public List<CubeInfo> cubesInfo = new List<CubeInfo>();

    void CheckForChangingMat() {
        if (meshRenderer != null) {
            int satisfied = 0;
            foreach (CubeInfo cubeInfo in cubesInfo) {
                bool condition = false;
                switch (cubeInfo.comp) {
                    case Comparaison.Equals:
                        condition = cubeInfo.cubesIn == cubeInfo.needed;
                        break;
                    case Comparaison.AtLeast:
                        condition = cubeInfo.cubesIn >= cubeInfo.needed;
                        break;
                    case Comparaison.AtMost:
                        condition = cubeInfo.cubesIn <= cubeInfo.needed;
                        break;
                }
                satisfied += condition ? 1 : 0;
            }
            
            bool finalCondition = false;
            switch (operatorType) {
                case Operator.AND:
                    finalCondition = satisfied == cubesInfo.Count();
                    break;
                case Operator.OR:
                    finalCondition = satisfied > 0;
                    break;
                case Operator.XOR:
                    finalCondition = satisfied == 1;
                    break;
            }

            if (finalCondition) {
                if (meshRenderer.materials.Count() == 0 || meshRenderer.materials[0] != outMaterial) {
                    originalMaterial = meshRenderer.materials.Count() > 0 ? meshRenderer.materials[0] : null;
                    meshRenderer.materials = new Material[] { outMaterial };
                }
            } else {
                if (originalMaterial != null)
                    meshRenderer.materials = new Material[] { originalMaterial };
                originalMaterial = null;
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        CheckForChangingMat();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerEnter(Collider other) {
        if(activated && other.TryGetComponent(out Cube cube)) {
            for (int i = cubesInfo.Count() - 1; i >= 0; i--) {
                CubeInfo cubeInfo = cubesInfo[i];
                if (cubeInfo.color == 0 || cube.cubeColor == cubeInfo.color) {
                    cubeInfo.cubesIn += cube.cubeSize;
                }
                cubesInfo[i] = cubeInfo;
            }
            CheckForChangingMat();
        }
    }

    void OnTriggerExit(Collider other) {
        if(activated && other.TryGetComponent(out Cube cube)) {
            for (int i = cubesInfo.Count() - 1; i >= 0; i--) {
                CubeInfo cubeInfo = cubesInfo[i];
                if (cubeInfo.color == 0 || cube.cubeColor == cubeInfo.color) {
                    cubeInfo.cubesIn -= cube.cubeSize;
                }
                cubesInfo[i] = cubeInfo;
            }
            CheckForChangingMat();
        }
    }
}
