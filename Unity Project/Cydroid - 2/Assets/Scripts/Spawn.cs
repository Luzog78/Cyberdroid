using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawn : MonoBehaviour {

    public bool activated = true;
    [Range(-1, 50)] public short room = -1;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerEnter(Collider collider) {
        if (activated) {
            if (collider.gameObject.TryGetComponent(out PlayerHandler player)) {
                player.spawn = this;
                if (Manager.instance.room != room) {
                    Manager.RoomHandler from = Manager.instance.GetRoomHandler(Manager.instance.room);
                    Manager.instance.room = room;
                    Manager.RoomHandler to = Manager.instance.GetRoomHandler(room);
                    if (from.room != -1) {
                        from.OnExit();
                    }
                    if (to.room != -1) {
                        to.OnEnter();
                    }
                }
            }
        }
    }
}
