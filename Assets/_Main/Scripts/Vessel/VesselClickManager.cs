using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselClickManager : MonoBehaviour
{
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null) {

                Debug.Log(hit.collider.gameObject.name);

                RoomManager roomManager;
                hit.collider.TryGetComponent(out roomManager);

                if(roomManager != null) {
                    if(roomManager.isZoomed) return;
                    VesselManager.instance.animator.SetTrigger($"Room{roomManager.roomIndex}");
                    VesselManager.instance.zoomedRoomManager = roomManager;
                }
            }
        }
    }
}
