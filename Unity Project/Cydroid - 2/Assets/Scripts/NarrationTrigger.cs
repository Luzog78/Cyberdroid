using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NarrationTrigger : MonoBehaviour {

    public bool activated = true;
    public string narration = null;
    public bool cooldowning = false;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerEnter(Collider collider) {
        if (activated && !cooldowning) {
            if (collider.gameObject.TryGetComponent(out PlayerHandler player)) {
                Manager.instance.Narrate(narration);
                cooldowning = true;
                StartCoroutine(Cooldown());
            }
        }
    }
    
    IEnumerator Cooldown() {
        yield return new WaitForSeconds(1f);
        cooldowning = false;
    }
}
