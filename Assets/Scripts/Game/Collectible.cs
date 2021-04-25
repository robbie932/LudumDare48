using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public GameObject particle;
    public int value = 10;
    public float pickUpRadius = 1f;
    bool inRange;
    Transform target;

    private void OnTriggerEnter(Collider other)
    {
        inRange = true;
        target = other.transform;
    }

    private void Update()
    {
        if (!inRange)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, target.position, 0.6f);
        var dist = Vector3.Distance(transform.position, target.position);
        if (dist <= pickUpRadius)
        {
            particle.transform.parent = null;
            particle.SetActive(true);
            Destroy(particle, 2f);
            Destroy(gameObject);
            PlayerController.Score += value;
            CollectablesUI.instance.UpdateText(PlayerController.Score, value);
        }
    }
}
