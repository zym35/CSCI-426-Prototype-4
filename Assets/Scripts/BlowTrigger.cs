using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowTrigger : MonoBehaviour
{
    public List<Rigidbody> rigidbodyInRange;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Gem"))
        {
            rigidbodyInRange.Add(other.attachedRigidbody);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Gem"))
        {
            rigidbodyInRange.Remove(other.attachedRigidbody);
        }
    }
}
