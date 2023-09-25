using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowTrigger : MonoBehaviour
{
    public List<Rigidbody> gemInRange;
    public List<PlayerController> playerInRange;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem"))
        {
            gemInRange.Add(other.attachedRigidbody);
        }
        
        if (other.CompareTag("Player"))
        {
            playerInRange.Add(other.GetComponent<PlayerController>());
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gem"))
        {
            gemInRange.Remove(other.attachedRigidbody);
        }
        
        if (other.CompareTag("Player"))
        {
            playerInRange.Remove(other.GetComponent<PlayerController>());
        }
    }
}
