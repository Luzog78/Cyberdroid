using System.Collections;
using TMPro;
using UnityEngine;

public class CheatHandler : MonoBehaviour {
    public TMP_InputField inputWalk;
    public TMP_InputField inputJump;
    public TMP_InputField inputRun;
    public TMP_InputField inputMouseX;
    public TMP_InputField inputMouseY;
    public TMP_InputField inputGroundDistance;
    public TMP_InputField inputDeaths;

    public CheckBox checkCollision;
    public CheckBox checkGravity;
    public CheckBox checkMortal;
    public CheckBox checkRotate;
    public CheckBox checkWalk;
    public CheckBox checkJump;
    public CheckBox checkRun;
    public CheckBox checkInteract;
    public CheckBox checkGrab;
    public CheckBox checkBreak;
    public CheckBox checkReset;
    public CheckBox checkExit;

    public void HandleFloat(TMP_InputField input) {
        //StartCoroutine(HandleFloatCoroutine(input));
    }

    IEnumerator HandleFloatCoroutine(TMP_InputField input) {
        yield return new WaitForSeconds(0.01f);
        string value = input.text.Replace(',', '.');
        for (int i = 0; i < value.Length; i++) {
            if (value[i] < '0' || value[i] > '9' || value[i] != '.') {
                value = value.Remove(i, 1);
                i--;
            }
        }
        input.text = value;
    }

    public void Refresh() {
        StartCoroutine(RefreshCoroutine());
    }

    IEnumerator RefreshCoroutine() {
        yield return new WaitForSeconds(0.02f);
        inputDeaths.text = Manager.instance.deaths.ToString();
        Manager.instance.players.ForEach((player) => {
            inputWalk.text = player.speed.ToString();
            inputJump.text = player.jump.ToString();
            inputRun.text = player.runningSpeed.ToString();
            inputMouseX.text = player.mouseSensitivity.x.ToString();
            inputMouseY.text = player.mouseSensitivity.y.ToString();
            inputGroundDistance.text = player.groundDistance.ToString();
            checkCollision.isChecked = player.cheatCanCollide;
            checkGravity.isChecked = player.cheatHasGravity;
            checkMortal.isChecked = player.cheatCanDie;
            checkRotate.isChecked = player.canRotate;
            checkWalk.isChecked = player.canWalk;
            checkJump.isChecked = player.canJump;
            checkRun.isChecked = player.canRun;
            checkInteract.isChecked = player.canInteract;
            checkGrab.isChecked = player.canGrab;
            checkBreak.isChecked = player.canBreak;
            checkReset.isChecked = player.canReset;
            checkExit.isChecked = player.canExit;
        });
    }

    public void ApplyInputs() {
        StartCoroutine(ApplyInputsCoroutine());
    }

    IEnumerator ApplyInputsCoroutine() {
        yield return new WaitForSeconds(0.01f);
        try {
            Manager.instance.deaths = int.Parse(inputDeaths.text);
        } catch (System.Exception) {
        }
        Manager.instance.players.ForEach((player) => {
            try {
                player.speed = float.Parse(inputWalk.text.Replace(',', '.'));
            } catch (System.Exception) {
            }
            try {
                player.jump = float.Parse(inputJump.text.Replace(',', '.'));
            } catch (System.Exception) {
            }
            try {
                player.runningSpeed = float.Parse(inputRun.text.Replace(',', '.'));
            } catch (System.Exception) {
            }
            try {
                player.mouseSensitivity.x = float.Parse(inputMouseX.text.Replace(',', '.'));
            } catch (System.Exception) {
            }
            try {
                player.mouseSensitivity.y = float.Parse(inputMouseY.text.Replace(',', '.'));
            } catch (System.Exception) {
            }
            try {
                player.groundDistance = float.Parse(inputGroundDistance.text.Replace(',', '.'));
            } catch (System.Exception) {
            }
        });
    }

    public void ApplyChecks() {
        StartCoroutine(ApplyChecksCoroutine());
    }

    IEnumerator ApplyChecksCoroutine() {
        yield return new WaitForSeconds(0.01f);
        Manager.instance.players.ForEach((player) => {
            player.cheatCanCollide = checkCollision.isChecked;
            player.cheatHasGravity = checkGravity.isChecked;
            player.cheatCanDie = checkMortal.isChecked;
            player.canRotate = checkRotate.isChecked;
            player.canWalk = checkWalk.isChecked;
            player.canJump = checkJump.isChecked;
            player.canRun = checkRun.isChecked;
            player.canInteract = checkInteract.isChecked;
            player.canGrab = checkGrab.isChecked;
            player.canBreak = checkBreak.isChecked;
            player.canReset = checkReset.isChecked;
            player.canExit = checkExit.isChecked;
        });
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
