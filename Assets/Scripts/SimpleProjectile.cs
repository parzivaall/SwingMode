using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float lifetime = 3f;    // How long until projectile destroys itself
    
    void Start()
    {
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // If hits player, destroy projectile
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

