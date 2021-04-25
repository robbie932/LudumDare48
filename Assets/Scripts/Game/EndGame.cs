using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        //Kinda redundant if-statement because all objects are static
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            Debug.Log("Dead");
        }
    }
}
