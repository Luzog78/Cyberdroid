using System;
using UnityEngine;
using System.Collections.Generic;

public class ControlUIHandler : MonoBehaviour {
    [Serializable]
    public enum Control {
        JUMP, RUN, FORWARD, BACKWARD, LEFT, RIGHT, INTERACT, RESET, RESET_ROTATION, EXIT, NONE
    }

    public bool activated = false;
    public Control currentControl = Control.NONE;
    public List<KeyCode> keys = new List<KeyCode>();

    public void SetControl(String control) {
        Debug.Log(control);
        currentControl = (Control) Enum.Parse(typeof(Control), control);
        Debug.Log(currentControl);
    }

    public void SetControl(Control control) {
        currentControl = control;
    }

    public void ResetControl() {
        keys.Clear();
        currentControl = Control.NONE;
    }

    void onStart() {

    }

    void Update() {
        if (!activated || currentControl == Control.NONE) return;

        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKey(kcode)) {
                if (!keys.Contains(kcode)) {
                    keys.Add(kcode);
                    Manager.instance.players.ForEach(player => {
                        switch (currentControl) {
                            case Control.JUMP:
                                player.keyJump.keys = new List<KeyCode>(keys);
                                break;
                            case Control.RUN:
                                player.keyRun.keys = new List<KeyCode>(keys);
                                break;
                            case Control.FORWARD:
                                player.keyForward.keys = new List<KeyCode>(keys);
                                break;
                            case Control.BACKWARD:
                                player.keyBackward.keys = new List<KeyCode>(keys);
                                break;
                            case Control.LEFT:
                                player.keyLeft.keys = new List<KeyCode>(keys);
                                break;
                            case Control.RIGHT:
                                player.keyRight.keys = new List<KeyCode>(keys);
                                break;
                            case Control.INTERACT:
                                player.keyInteract.keys = new List<KeyCode>(keys);
                                break;
                            case Control.RESET:
                                player.keyReset.keys = new List<KeyCode>(keys);
                                break;
                            case Control.RESET_ROTATION:
                                player.keyResetRotation.keys = new List<KeyCode>(keys);
                                break;
                            case Control.EXIT:
                                player.keyExit.keys = new List<KeyCode>(keys);
                                break;
                        }
                    });
                }
            } else if (keys.Count > 0 && keys.Contains(kcode)) {
                ResetControl();
                break;
            }
        }
    }
}