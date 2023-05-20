using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityMotor : MonoBehaviour
{

    public bool activated = true, rotate = true;
    public List<GravityArea> gravityAreas = new List<GravityArea>();

    private Rigidbody rb;

    [ReadOnly] public Vector3 gravity;

    [SerializeField] Vector3 Gravity {
        get
        {
            if (gravityAreas.Count == 0)
                return Vector3.zero;
            gravityAreas.Sort((area1, area2) => area1.priority.CompareTo(area2.priority));
            GravityArea area = gravityAreas.Last();
            return area.gravityDirection * area.gravityStrength;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gravity = Gravity;
        if (activated) {

            if (rotate)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                Quaternion rotation = Quaternion.FromToRotation(transform.up, -gravity) * transform.rotation;
                rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
                transform.rotation = rotation;
            } else {
                rb.constraints = RigidbodyConstraints.None;
            }

            rb.AddForce(gravity * rb.mass * Time.deltaTime, ForceMode.Acceleration);
        }
    }
}
