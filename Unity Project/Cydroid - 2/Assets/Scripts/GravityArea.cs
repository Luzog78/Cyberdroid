using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GravityArea : MonoBehaviour
{

    [Header("Don't forget to turn on the isTrigger flag of the collider")]
    [Space(25)]

    public bool activated = true;
    public int priority = 0;
    public float gravityStrength = 900f;
    public Vector3 gravityDirection = Vector3.down;

    private Collider col;

    void Start()
    {
        col = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out GravityMotor motor))
        {
            motor.gravityAreas.Remove(this);
            if(activated)
                motor.gravityAreas.Add(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GravityMotor motor))
        {
            motor.gravityAreas.Remove(this);
        }
    }
}
