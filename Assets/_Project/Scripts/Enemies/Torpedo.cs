using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public float speed = 10f;  // Speed of the torpedo
    private Transform target;  // Target to move towards

    [SerializeField] ParticleSystem Explosion;
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

    private void OnTriggerEnter(Collider other)

    {
        // Check if the torpedo collides with the camera or other objects
        if (other.gameObject.CompareTag("MainCamera"))
        {
            // Destroy the torpedo after hitting the camera
            Destroy(gameObject);
        }else 

        if (other.CompareTag("PlayerProjectile"))
        {
            StartCoroutine(Explote());
        }

    }

    private IEnumerator Explote()
    {
        Debug.LogWarning("uwu");
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Explosion.Play(); 
        yield return new WaitForSeconds(Explosion.main.duration);
        Destroy(gameObject);
    }

}
