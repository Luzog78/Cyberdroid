using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBreakHandler : MonoBehaviour {

    public bool activated = true;
    public Breakable breakable;
    public List<PlayerHandler> players = new List<PlayerHandler>();

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        foreach (PlayerHandler player in players) {
            if (player.tryingInteract) {
                //breakable.broken = true;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out PlayerHandler player)) {
            players.Add(player);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out PlayerHandler player)) {
            players.Remove(player);
        }
    }
}
