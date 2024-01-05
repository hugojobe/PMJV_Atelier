using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FixedConnectorUnspeed : MonoBehaviour
{
    private DecoReco decoReco => GetComponentInParent<DecoReco>();

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("MoveableconnectorToRemove")){
            other.TryGetComponent(out MovingConnectorUnspeed component);
            component.connected = true;
            decoReco.readyToConnect = false;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("MoveableconnectorToRemove")){
            other.TryGetComponent(out MovingConnectorUnspeed component);
            component.connected = false;
            decoReco.readyToConnect = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("MovableConnector")) {
            other.TryGetComponent(out MovingConnectorUnspeed connector);
            connector.interactible = false;
            connector.ForceEndDrag();
            connector.rectTransform.localPosition = connector.connectedPosition;
            decoReco.finished = true;
        }
    }
}
