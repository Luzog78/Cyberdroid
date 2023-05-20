using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OnlyObjectCanPass : MonoBehaviour {

    [Header("This is a final list. Setted up before start.")]
    public List<GameObject> ignoredObjects = new List<GameObject>();

    private List<Collider> colliders = new List<Collider>();

    // Start is called before the first frame update
    void Start() {
        foreach(Collider c in GetComponents<Collider>()) {
            if (c.enabled) {
                colliders.Add(c);
            }
        }
        ignoredObjects.ForEach((o) => {
            List<Collider> activeColliders = new List<Collider>();
            foreach (Collider c in o.GetComponents<Collider>()) {
                if (c.enabled)
                    activeColliders.Add(c);
            }
            foreach (Collider c in o.GetComponentsInChildren<Collider>()) {
                if (c.enabled)
                    activeColliders.Add(c);
            }
            activeColliders.ForEach((c1) => {
                colliders.ForEach((c2) => {
                    if (!c1.isTrigger && !c2.isTrigger)
                        Physics.IgnoreCollision(c1, c2);
                });
            });
        });
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnCollisionEnter(Collision collision) {

    }
}
