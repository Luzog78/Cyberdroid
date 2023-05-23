using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHandler : MonoBehaviour {
    [Serializable]
    public enum PressType {
        Pressed,
        Down,
        Up
    }

    [Serializable]
    public struct Key {
        public bool ui;
        public List<KeyCode> keys;
        public PressType pressType;

        public Key(bool ui, PressType pressType, params KeyCode[] key) {
            this.ui = ui;
            this.keys = new List<KeyCode>(key);
            this.pressType = pressType;
        }

        public bool IsPressed() {
            PressType pressType = this.pressType;
            return keys.Any(k => (pressType == PressType.Pressed && Input.GetKey(k))
                              || (pressType == PressType.Down    && Input.GetKeyDown(k))
                              || (pressType == PressType.Up      && Input.GetKeyUp(k)));
        }

        public void SetVisibleInUI(bool ui) {
            this.ui = ui;
        }
    }

    public bool activated = true;
    
    [Space(10)]
    [Header("Settings Constants")]
    [Space(10)]
    public float speed = 10f;
    public float runningSpeed = 15f;
    public float jump = 6f;
    [Space(10)]
    public Vector2 mouseSensitivity = new Vector2(150f, 150f);
    [Space(10)]
    public float groundDistance = 1.1f;
    [Space(10)]
    public GameObject cameraAnchor;
    public Animator playerAnimator;
    public Material raycastMaterial;
    public Material resetMaterial;
    public Material dieMaterial;
    
    [Space(10)]
    [Header("Cheats")]
    [Space(10)]
    public bool cheatCanCollide = true;
    public bool cheatHasGravity = true;
    public bool cheatCanDie = true;
    
    [Space(10)]
    [Header("Unlockables")]
    [Space(10)]
    public bool canRotate = true;
    public bool canWalk = true;
    public bool canRun = false;
    public bool canJump = false;
    public bool canInteract = false;
    public bool canGrab = false;
    public bool canBreak = false;
    public bool canReset = false;
    public bool canExit = true;

    [Space(10)]
    [Header("Automatic Variables")]
    [Space(10)]
    public Spawn spawn;
    public Rigidbody grabbingObject;
    public PhysicMaterial grabbingMaterial;
    [Space(10)]
    public bool isGrounded;
    public bool isJumping;
    public bool isInteracting;
    [Space(10)]
    public Key keyJump          = new Key(true, PressType.Pressed, KeyCode.Space                 );
    public Key keyRun           = new Key(true, PressType.Pressed, KeyCode.LeftShift             );
    public Key keyForward       = new Key(true, PressType.Pressed, KeyCode.Z                     );
    public Key keyBackward      = new Key(true, PressType.Pressed, KeyCode.S                     );
    public Key keyLeft          = new Key(true, PressType.Pressed, KeyCode.Q                     );
    public Key keyRight         = new Key(true, PressType.Pressed, KeyCode.D                     );
    public Key keyInteract      = new Key(true, PressType.Down,    KeyCode.Mouse0, KeyCode.Mouse1);
    public Key keyReset         = new Key(true, PressType.Down,    KeyCode.R                     );
    public Key keyResetRotation = new Key(true, PressType.Down,    KeyCode.T                     );
    public Key keyExit          = new Key(true, PressType.Down,    KeyCode.Escape                );
    [Space(10)]
    public Vector2 mouseInput;
    [Space(10)]
    public bool tryingJump;
    public bool tryingRun;
    public bool tryingForward;
    public bool tryingBackward;
    public bool tryingLeft;
    public bool tryingRight;
    public bool tryingInteract;
    public bool tryingReset;
    public bool tryingResetRotation;
    public bool tryingExit;

    private Rigidbody rb;

    public void CheckGrounded() {
        isGrounded = Physics.Raycast(rb.transform.position, -rb.transform.up, groundDistance, 1);
    }

    private void UpdateUiField(ref Manager.UIControl uiControl, Key? key) {
        if (key == null || !((Key) key).ui) {
            if (uiControl.textBox != null) {
                uiControl.textBox.text = "";
            }
            uiControl.otherObjects.ForEach(o => o.SetActive(false));
        } else {
            if (uiControl.textBox != null) {
                string text = ((Key) key).keys.Select(k => k.ToString())
                                              .Aggregate((a, b) => a + "\n" + b);
                text = text.ToUpper().Replace("LEFT", "L").Replace("RIGHT", "R")
                                     .Replace("ESCAPE", "ESC").Replace("DELETE", "DEL")
                                     .Replace("MOUSE", "M ").Replace("KEYPAD", "KPAD ");
                uiControl.textBox.text = text;
            }
            uiControl.otherObjects.ForEach(o => o.SetActive(true));
        }
    }

    public void UpdateUi() {
        UpdateUiField(ref Manager.instance.uiControlJump, canJump ? keyJump : null);
        UpdateUiField(ref Manager.instance.uiControlRun, canRun ? keyRun : null);
        UpdateUiField(ref Manager.instance.uiControlForward, canWalk ? keyForward : null);
        UpdateUiField(ref Manager.instance.uiControlBackward, canWalk ? keyBackward : null);
        UpdateUiField(ref Manager.instance.uiControlLeft, canWalk ? keyLeft : null);
        UpdateUiField(ref Manager.instance.uiControlRight, canWalk ? keyRight : null);
        UpdateUiField(ref Manager.instance.uiControlInteract, canInteract ? keyInteract : null);
        UpdateUiField(ref Manager.instance.uiControlReset, canReset ? keyReset : null);
        UpdateUiField(ref Manager.instance.uiControlExit, canExit ? keyExit : null);
    }

    public void ActiveProperty(String property) {
        switch (property.ToLower()) {
            case "rotate":
                canRotate = true;
                break;
            case "walk":
                canWalk = true;
                break;
            case "run":
                canRun = true;
                break;
            case "jump":
                canJump = true;
                break;
            case "interact":
                canInteract = true;
                break;
            case "grab":
                canGrab = true;
                break;
            case "break":
                canBreak = true;
                break;
            case "reset":
                canReset = true;
                break;
            case "exit":
                canExit = true;
                break;
            default:
                Debug.LogError("Unknown property: " + property);
                break;
        }
    }

    public void DeactiveProperty(String property) {
        switch (property.ToLower()) {
            case "rotate":
                canRotate = false;
                break;
            case "walk":
                canWalk = false;
                break;
            case "run":
                canRun = false;
                break;
            case "jump":
                canJump = false;
                break;
            case "interact":
                canInteract = false;
                break;
            case "grab":
                canGrab = false;
                break;
            case "break":
                canBreak = false;
                break;
            case "reset":
                canReset = false;
                break;
            case "exit":
                canExit = false;
                break;
            default:
                Debug.LogError("Unknown property: " + property);
                break;
        }
    }

    void Awake() {
        if (FindObjectsOfType<PlayerHandler>().Length > 1) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        CheckGrounded();
    }

    // Update is called once per frame
    void Update() {
        UpdateUi();

        if (activated) {
            CheckGrounded();

            mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            tryingJump          = keyJump         .IsPressed();
            tryingRun           = keyRun          .IsPressed();
            tryingForward       = keyForward      .IsPressed();
            tryingBackward      = keyBackward     .IsPressed();
            tryingLeft          = keyLeft         .IsPressed();
            tryingRight         = keyRight        .IsPressed();
            tryingInteract      = keyInteract     .IsPressed();
            tryingReset         = keyReset        .IsPressed();
            tryingResetRotation = keyResetRotation.IsPressed();
            tryingExit          = keyExit         .IsPressed();

            if (canExit && tryingExit) {
                Manager.instance.TogglePause();
                return;
            }

            if (Manager.instance.window != Manager.Window.GAME) {
                mouseInput = Vector2.zero;
                tryingJump          = false;
                tryingRun           = false;
                tryingForward       = false;
                tryingBackward      = false;
                tryingLeft          = false;
                tryingRight         = false;
                tryingInteract      = false;
                tryingReset         = false;
                tryingResetRotation = false;
            }

            // --- Handle Reset ---

            if (canReset && tryingReset) {
                //TryToReset();
                Die();
                return;
            }

            // --- Handle Colliders ---

            foreach(Collider c in GetComponents<Collider>()) {
                c.enabled = cheatCanCollide;
            }

            // --- Handle Movement & Animations ---

            float finalSpeed = canWalk ? speed : 0;
            if (canRun && tryingRun) {
                finalSpeed = runningSpeed;
            }

            float reduction = isGrounded ? 1 : 0.2f;

            if (canJump && tryingJump) {
                if (isJumping) {
                    rb.AddForce(rb.transform.up * jump, ForceMode.Acceleration);
                } else if (isGrounded) {
                    rb.velocity *= 0.2f;
                    rb.AddForce(rb.transform.up * jump * 0.5f, ForceMode.Impulse);
                    isJumping = true;
                    StartCoroutine(JumpCooldown());
                }
            }

            float physics = finalSpeed * reduction;
            if (physics > 0) {
                if (tryingForward) {
                    rb.AddForce(rb.transform.forward * physics, ForceMode.Acceleration);
                }
                if (tryingBackward) {
                    rb.AddForce(-rb.transform.forward * physics, ForceMode.Acceleration);
                }
                if (tryingLeft) {
                    rb.AddForce(-rb.transform.right * physics, ForceMode.Acceleration);
                }
                if (tryingRight) {
                    rb.AddForce(rb.transform.right * physics, ForceMode.Acceleration);
                }

                float maxSpeed = finalSpeed * (isGrounded ? 1 : 2.5f);
                if(rb.velocity.magnitude > maxSpeed) {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
            }

            if (playerAnimator != null) {
                playerAnimator.SetFloat("Speed", rb.velocity.magnitude);
                playerAnimator.SetBool("Jump", isJumping);
                playerAnimator.SetBool("Grounded", isGrounded);
                playerAnimator.SetBool("FreeFall", !isGrounded);
                playerAnimator.SetFloat("MotionSpeed", 1);
            }

            // --- Handle Camera & Rotation ---

            if (canRotate) {
                if (tryingResetRotation) {
                    transform.rotation = Quaternion.identity;
                    if(cameraAnchor != null)
                        cameraAnchor.transform.localEulerAngles = Vector3.zero;
                }
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
            }

            // --- Handle Interactions ---

            Vector3 from = cameraAnchor.transform.position + transform.up;

            if (grabbingObject != null) {
                grabbingObject.velocity = Vector3.zero;
                grabbingObject.AddForce((from + cameraAnchor.transform.forward * 8)
                                        - grabbingObject.transform.position, ForceMode.Impulse);
            }

            if (tryingInteract) {
                TryToInteract();
            }
        }
    }

    public void Die() {
        if (spawn != null && cheatCanDie) {
            Transform t = spawn.spawnPoint == null ? spawn.transform : spawn.spawnPoint;
            List<Renderer> activeRenderers = GetActiveRenderers();
            List<Collider> activeColliders = GetActiveColliders();
            foreach(Renderer r in activeRenderers) {
                r.enabled = false;
            }
            foreach(Collider c in activeColliders) {
                c.enabled = false;
            }
            canReset = false;
            bool _canDie = cheatCanDie;
            cheatCanDie = false;
            Missile.Create(
                origin: transform.position,
                target: t.position,
                breakable: false,
                material: dieMaterial,
                onUpdate: (missile) => {
                    transform.position = missile.transform.position;
                },
                onReachTarget: (missile) => {
                    if (TryGetComponent(out GravityMotor gm)) {
                        gm.gravityAreas.Clear();
                    }
                    transform.position = t.position;
                    transform.rotation = t.rotation;
                    rb.velocity = Vector3.zero;
                    if(cameraAnchor != null)
                        cameraAnchor.transform.localEulerAngles = Vector3.zero;
                    foreach(Renderer r in activeRenderers) {
                        r.enabled = true;
                    }
                    foreach(Collider c in activeColliders) {
                        c.enabled = true;
                    }
                    canReset = true;
                    cheatCanDie = _canDie;
                    Manager.instance.deaths++;
                    Manager.ResetGame();
                }
            );
        }
    }

    public List<Renderer> GetActiveRenderers() {
        List<Renderer> renderers = new List<Renderer>();
        foreach(Renderer r in GetComponents<Renderer>()) {
            if (r.enabled) {
                renderers.Add(r);
            }
        }
        foreach(Renderer r in GetComponentsInChildren<Renderer>()) {
            if (r.enabled) {
                renderers.Add(r);
            }
        }
        return renderers;
    }

    public List<Collider> GetActiveColliders() {
        List<Collider> colliders = new List<Collider>();
        foreach(Collider c in GetComponents<Collider>()) {
            if (c.enabled) {
                colliders.Add(c);
            }
        }
        foreach(Collider c in GetComponentsInChildren<Collider>()) {
            if (c.enabled) {
                colliders.Add(c);
            }
        }
        return colliders;
    }

    public void TryToReset() {
        if (canReset) {
            if (spawn != null) {
                Transform t = spawn.spawnPoint == null ? spawn.transform : spawn.spawnPoint;
                Missile.Create(
                    origin: transform.position,
                    target: t.position,
                    material: resetMaterial,
                    onReachTarget: (missile) => {
                        transform.position = t.position;
                        transform.rotation = t.rotation;
                        Manager.ResetGame();
                    }
                );
            }
        }
    }

    public void TryToInteract() {
        if (canInteract && !isInteracting) {
            isInteracting = true;
            StartCoroutine(InteractionCooldown());
            if (grabbingObject == null) {
                Vector3 from = cameraAnchor.transform.position + transform.up;
                Missile.Create(
                    origin: from,
                    target: from + cameraAnchor.transform.forward * 16,
                    material: raycastMaterial,
                    onCollide: (missile, collider) => {
                        if (collider.gameObject.TryGetComponent(out Cube cube)) {
                            grabbingObject = cube.gameObject.GetOrAddComponent<Rigidbody>();
                            Collider c = grabbingObject.gameObject.GetComponent<Collider>();
                            grabbingMaterial = c.material;
                            c.material = Manager.instance.noFriction;
                            Destroy(missile.gameObject);
                        }
                        if (collider.gameObject.TryGetComponent(out DoorBreakHandler doorBreakable)) {
                            if (doorBreakable.breakable != null) {
                                doorBreakable.breakable.broken = true;
                            }
                            Destroy(missile.gameObject);
                        }
                    }
                );
            } else {
                if (grabbingMaterial != null) {
                    grabbingObject.gameObject.GetComponent<Collider>().material = grabbingMaterial;
                }
                grabbingObject = null;
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
