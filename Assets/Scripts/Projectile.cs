using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rbody;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rbody.AddForce(direction * force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collision if it hits the player so it doesn't self-destruct on spawn
        if (collision.CompareTag("Player"))
        {
            return;
        }

        // Destroy on hitting anything else (enemies, walls, etc.)
        Destroy(gameObject);
    }
}