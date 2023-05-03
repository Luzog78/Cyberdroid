using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Killer : MonoBehaviour {

    public bool activated = true;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerEnter(Collider collider) {
        if (activated) {
            if (collider.gameObject.TryGetComponent(out PlayerHandler player)) {
                player.Die();
            }
            if (collider.gameObject.TryGetComponent(out Missile missile)) {
                missile.Break();
            }
        }
    }
}
