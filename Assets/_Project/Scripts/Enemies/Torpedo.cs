using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public float speed = 15f;  // Speed of the torpedo
    private Transform _target;  // Target to move towards
    private Vector3 _direction;
    private SoundManager _soundManager;
    private int _damage = 5;
    [SerializeField] ParticleSystem Fire;
    [SerializeField] AudioSource audioSource;

    [SerializeField] ParticleSystem Explosion;

    public int Damage { get => _damage; set => _damage = value; }




    void Start()
    {
        // Find the player's main camera as the _target
        _target = Camera.main.transform;
        _soundManager = SoundManager.Instance;  
        if (_target != null)
        {
            // Calculate _direction towards the camera
            _direction = _target.position - transform.position;
            _direction.Normalize();  // Normalize the _direction to get only the _direction (not magnitude)

            // Move the torpedo towards the camera
            transform.position += _direction * speed * Time.deltaTime;

            // Optional: Rotate the torpedo to face the camera
            transform.rotation = Quaternion.LookRotation(_direction);
        }
    }

    void Update()
    {
        if (_target != null)
        {
            // Move the torpedo towards the camera
            transform.position += _direction * speed * Time.deltaTime;

            // Optional: Rotate the torpedo to face the camera
            transform.rotation = Quaternion.LookRotation(_direction);

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
        _soundManager.PlaySound("Torpedo", audioSource);
        Fire.Stop();
        speed = 0;
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Explosion.Play(); 
        yield return new WaitForSeconds(Explosion.main.duration);
        Destroy(gameObject);
    }



}
