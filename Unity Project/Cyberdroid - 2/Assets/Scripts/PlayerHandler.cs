using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHandler : MonoBehaviour
{

    public bool activated = true;
    public float speed = 10f, runningSpeed = 15f;
    public float jump = 6f;
    public Vector2 mouseSensitivity = new Vector2(150f, 150f);
    public float groundDistance = 1.1f;
    public bool isGrounded, isJumping;

    public GameObject cameraAnchor;

    private Rigidbody rb;

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(rb.transform.position, -rb.transform.up, groundDistance, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CheckGrounded();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated) {
            CheckGrounded();
            if (Input.GetKey(KeyCode.Space) && isGrounded && !isJumping) {
                rb.AddForce(rb.transform.up * jump, ForceMode.Impulse);
                isJumping = true;
                StartCoroutine(JumpCooldown());
            }

            float finalSpeed = Input.GetKey(KeyCode.F) ? runningSpeed : speed;

            if (Input.GetKey(KeyCode.S)) {
                rb.AddForce(rb.transform.forward * finalSpeed, ForceMode.Acceleration);
            }
            if (Input.GetKey(KeyCode.W)) {
                rb.AddForce(-rb.transform.forward * finalSpeed, ForceMode.Acceleration);
            }
            if (Input.GetKey(KeyCode.Q)) {
                rb.AddForce(-rb.transform.right * finalSpeed, ForceMode.Acceleration);
            }
            if (Input.GetKey(KeyCode.D)) {
                rb.AddForce(rb.transform.right * finalSpeed, ForceMode.Acceleration);
            }
            /*if (Input.GetKey(KeyCode.A)) {
                transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D)) {
                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            }*/

            if(rb.velocity.magnitude > finalSpeed) {
                rb.velocity = rb.velocity.normalized * finalSpeed;
            }

            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseSensitivity.y * Time.deltaTime);
            if(cameraAnchor != null) {
                cameraAnchor.transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * mouseSensitivity.x * Time.deltaTime);
                float rot = cameraAnchor.transform.localEulerAngles.x;
                if (rot > 60 && rot < 290)
                {
                    // 60 >= x° >= 0 || 360 >= x° >= 290
                    bool tooLow = Mathf.Abs(rot - 60) < Mathf.Abs(rot - 290);
                    cameraAnchor.transform.localEulerAngles = new Vector3(tooLow ? 60 : 290, 0, 0);
                }
            }
        }
    }

    IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.1f);
        isJumping = false;
    }
}
