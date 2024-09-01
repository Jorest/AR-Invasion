using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public float speed = 10f;  // Speed of the torpedo
    private Transform target;  // Target to move towards

    void Start()
    {
        // Find the player's main camera as the target
        target = Camera.main.transform;
    }

    void Update()
    {
        if (target != null)
        {
            // Calculate direction towards the camera
            Vector3 direction = target.position - transform.position;
            direction.Normalize();  // Normalize the direction to get only the direction (not magnitude)

            // Move the torpedo towards the camera
            transform.position += direction * speed * Time.deltaTime;

            // Optional: Rotate the torpedo to face the camera
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    // Optional: Handle collision with the camera or other objects
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the torpedo collides with the camera or other objects
        if (collision.gameObject.CompareTag("MainCamera"))
        {
            // Implement what happens when the torpedo hits the camera
            Debug.LogWarning("Torpedo hit the camera!");

            // Destroy the torpedo after hitting the camera
            Destroy(gameObject);
        }
    }
}
