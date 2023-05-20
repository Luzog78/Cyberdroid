using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OnlyPlayerCanPass : MonoBehaviour {

    private List<Collider> colliders = new List<Collider>();
    private List<Collider> ignoredColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start() {
        foreach(Collider c in GetComponents<Collider>()) {
            if (c.enabled) {
                colliders.Add(c);
            }
        }
        Manager.instance.players.ForEach((p) => {
            p.GetActiveColliders().ForEach((c) => {
                if (!c.isTrigger)
                    ignoredColliders.Add(c);
            });
        });
        ignoredColliders.ForEach((c1) => {
            colliders.ForEach((c2) => {
                if (!c1.isTrigger && !c2.isTrigger)
                    Physics.IgnoreCollision(c1, c2);
            });
        });
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent(out PlayerHandler player)) {
            player.GetActiveColliders().ForEach((c1) => {
                if (!c1.isTrigger && !ignoredColliders.Contains(c1)) {
                    ignoredColliders.Add(c1);
                    colliders.ForEach((c2) => {
                        if (!c2.isTrigger)
                            Physics.IgnoreCollision(c1, c2);
                    });
                }
            });
        }
    }
}
