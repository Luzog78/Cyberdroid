using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHandler : MonoBehaviour {

    public bool activated = true;
    public float speed = 10f, runningSpeed = 15f;
    public float jump = 6f;
    public Vector2 mouseSensitivity = new Vector2(150f, 150f);
    public float groundDistance = 1.1f;
    public GameObject cameraAnchor;
    public Material raycastMaterial;
    public bool isGrounded, isJumping, isInteracting;
    public Rigidbody grabbingObject;
    public bool tryingJump, tryingRun, tryingForward, tryingBackward, tryingLeft, tryingRight, tryingInteract;
    public Vector2 mouseInput;

    private Rigidbody rb;

    void CheckGrounded() {
        isGrounded = Physics.Raycast(rb.transform.position, -rb.transform.up, groundDistance, 1);
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        CheckGrounded();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        if (activated) {
            CheckGrounded();

            tryingRun = Input.GetKey(KeyCode.F);
            tryingJump = Input.GetKey(KeyCode.Space);
            tryingForward = Input.GetKey(KeyCode.S);
            tryingBackward = Input.GetKey(KeyCode.W);
            tryingLeft = Input.GetKey(KeyCode.Q);
            tryingRight = Input.GetKey(KeyCode.D);
            tryingInteract = Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
            mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            // --- Handle Movement ---

            float finalSpeed = tryingRun ? runningSpeed : speed;
            float reduction = isGrounded ? 1 : 0.33f;

            if (tryingJump) {
                if (isJumping) {
                    rb.AddForce(rb.transform.up * jump, ForceMode.Acceleration);
                } else if (isGrounded) {
                    isJumping = true;
                    StartCoroutine(JumpCooldown());
                }
            }

            if (tryingForward) {
                rb.AddForce(rb.transform.forward * finalSpeed * reduction, ForceMode.Acceleration);
            }
            if (tryingBackward) {
                rb.AddForce(-rb.transform.forward * finalSpeed * reduction, ForceMode.Acceleration);
            }
            if (tryingLeft) {
                rb.AddForce(-rb.transform.right * finalSpeed * reduction, ForceMode.Acceleration);
            }
            if (tryingRight) {
                rb.AddForce(rb.transform.right * finalSpeed * reduction, ForceMode.Acceleration);
            }

            if(rb.velocity.magnitude > finalSpeed) {
                rb.velocity = rb.velocity.normalized * finalSpeed;
            }

            // --- Handle Camera & Rotation ---

            transform.Rotate(Vector3.up, mouseInput.x * mouseSensitivity.y * Time.deltaTime);
            if(cameraAnchor != null) {
                cameraAnchor.transform.Rotate(Vector3.right, -mouseInput.y * mouseSensitivity.x * Time.deltaTime);
                float rot = cameraAnchor.transform.localEulerAngles.x;
                if (rot > 60 && rot < 290)
                {
                    // 60 >= x° >= 0 || 360 >= x° >= 290
                    bool tooLow = Mathf.Abs(rot - 60) < Mathf.Abs(rot - 290);
                    cameraAnchor.transform.localEulerAngles = new Vector3(tooLow ? 60 : 290, 0, 0);
                }
            }

            // --- Handle Interactions ---

            Vector3 distantLocation = cameraAnchor.transform.position + cameraAnchor.transform.forward * 6;

            if (grabbingObject != null) {
                grabbingObject.velocity = Vector3.zero;
                grabbingObject.AddForce(distantLocation - grabbingObject.transform.position, ForceMode.Impulse);
            }

            if (tryingInteract && !isInteracting) {
                isInteracting = true;
                StartCoroutine(InteractionCooldown());
                Missile.Create(origin: cameraAnchor.transform.position,
                               target: distantLocation,
                               material: raycastMaterial);

                if (grabbingObject == null) {
                    RaycastHit hit;
                    if (Physics.Raycast(cameraAnchor.transform.position, cameraAnchor.transform.forward, out hit, 10)) {
                        if (hit.collider.TryGetComponent(out Rigidbody rb)) {
                            // "cube" will not be used, but it is required to check if the object is a cube
                            if (rb.gameObject.TryGetComponent(out Cube cube)) {
                                grabbingObject = rb;
                            }
                        }
                    }
                } else {
                    grabbingObject = null;
                }
            }
        }
    }

    IEnumerator JumpCooldown() {
        yield return new WaitForSeconds(0.1f);
        isJumping = false;
    }

    IEnumerator InteractionCooldown() {
        yield return new WaitForSeconds(0.1f);
        isInteracting = false;
    }
}
