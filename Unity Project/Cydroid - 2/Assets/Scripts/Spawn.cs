using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawn : MonoBehaviour {

    public bool activated = true;
    [Range(-1, 50)] public short room = -1;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerEnter(Collider collider) {
        if (activated) {
            if (collider.gameObject.TryGetComponent(out PlayerHandler player)) {
                player.spawn = this;
            }
        }
    }
}
