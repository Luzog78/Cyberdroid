using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public Vector3 target;
    public float speed = 20f;

    public System.Action<Missile> onReachTarget = null;
    public System.Action<Missile, Collider> onCollide = null;

    public static Missile Create(Vector3 origin, Vector3 target,
                                 float speed = 20f, float radius = 0.2f, float length = 1f,
                                 Material material = null, System.Action<Missile> onReachTarget = null,
                                 System.Action<Missile, Collider> onCollide = null) {
        GameObject missile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        missile.transform.localScale = new Vector3(radius, radius, length);
        missile.transform.rotation = Quaternion.LookRotation(target - origin);
        missile.transform.position = origin;
        Destroy(missile.GetComponent<SphereCollider>());
        CapsuleCollider c = missile.AddComponent<CapsuleCollider>();
        c.isTrigger = true;
        c.radius = 0.5f;
        c.height = 1;
        c.direction = 2;
        if (material != null) {
            missile.GetComponent<MeshRenderer>().material = material;
        }
        Missile m = missile.AddComponent<Missile>();
        m.target = target;
        m.speed = speed;
        m.onReachTarget = onReachTarget;
        m.onCollide = onCollide;
        return m;
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) <= 0.0001f) {
            if (onReachTarget != null) {
                onReachTarget(this);
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (onCollide != null) {
            onCollide(this, other);
        }
    }

    void OnTriggerExit(Collider other) {

    }
}
