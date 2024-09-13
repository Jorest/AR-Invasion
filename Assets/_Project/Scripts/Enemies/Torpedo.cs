using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public float speed = 15f;  // Speed of the torpedo
    private Transform target;  // Target to move towards
    private Vector3 direction;

    private int _damage = 1;

    [SerializeField] ParticleSystem Explosion;

    public int Damage { get => _damage; set => _damage = value; }

    void Start()
    {
        // Find the player's main camera as the target
        target = Camera.main.transform;

        if (target != null)
        {
            // Calculate direction towards the camera
            direction = target.position - transform.position;
            direction.Normalize();  // Normalize the direction to get only the direction (not magnitude)

            // Move the torpedo towards the camera
            transform.position += direction * speed * Time.deltaTime;

            // Optional: Rotate the torpedo to face the camera
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Move the torpedo towards the camera
            transform.position += direction * speed * Time.deltaTime;

            // Optional: Rotate the torpedo to face the camera
            transform.rotation = Quaternion.LookRotation(direction);

        }
    }

    private void OnTriggerEnter(Collider other)

    {        
        if (other.CompareTag("PlayerProjectile"))
        {
            StartCoroutine(Explote());
        }

    }

    private IEnumerator Explote()
    {
        speed = 0;
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Explosion.Play(); 
        yield return new WaitForSeconds(Explosion.main.duration);
        Destroy(gameObject);
    }



}
