using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public GameObject particle;
    public int value = 10;

    private void OnTriggerEnter(Collider other)
    {
        particle.transform.parent = null;
        particle.SetActive(true);
        Destroy(particle, 2f);
        Destroy(gameObject);
        PlayerController.Score += value;
    }
}
