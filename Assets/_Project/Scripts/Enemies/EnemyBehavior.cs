using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    private bool _electrocuted = false;
    private bool _burned = false;
    private bool _frozen = false;
    private int _healtPoints = 4;
    private float _torPedoCoolDown = 5f;

    private Camera _camera;
    private float _rotationSpeed = 8f;
    private float _shootAnimationTime = 1.1f;
    private float _distanceFoward = 2f;
    private bool _fowardone = false;

    [SerializeField] private float StopDistance = 1f;
    [SerializeField] GameObject DamagedGameObject;
    [SerializeField] ParticleSystem Explosion;
    [SerializeField] MeshRenderer MainMesh;
    [SerializeField] GameObject TorpedoPrefab;




    private float yMovementRange = 0.1f;

    #region UnityDefault

    void Start()
    {
        StartCoroutine(MoveFoward());
        StartCoroutine(StartShooting());

    }

    void Update()
    {
        if (_fowardone)
        {
            RotateAroundTarget(_camera.transform, _rotationSpeed);
            MoveTowardsPlayer();
        }
    }

    private void OnEnable()
    {
        _camera = Camera.main;
    }

    #endregion
    private IEnumerator MoveFoward()
    {
        yield return new WaitForSeconds(0.3f);

        float elapsedTime = 0f;
        float duration = 1f;
        Vector3 initialPos = transform.position;
        Vector3 newPos = transform.position + (this.transform.forward * 0.5f);

        newPos.y += Random.Range(-yMovementRange, yMovementRange);


        Vector3 direction = _camera.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(initialPos, newPos, t);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, t);

            elapsedTime += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }
        _fowardone = true;
        // transform.position = new Vector3(0,0,0);
    }
        
    private IEnumerator StartShooting()
    {
        yield return new WaitForSeconds(1f);

        float _ogRotationSpeed = _rotationSpeed;

        while (_healtPoints > 0 ) { 
        
            // Lerp the _rotationSpeed to 0 over time
            float elapsedTime = 0f;
            float duration = 0.5f; // Adjust this value to control the speed of the lerp
            float initialRotationSpeed = _rotationSpeed;


            // nested while to lerp the speed to 0 
            while (elapsedTime < duration)
            {
                _rotationSpeed = Mathf.Lerp(initialRotationSpeed, 0f, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            _rotationSpeed = 0f; // Ensure it is exactly 0 after the lerp
            GameObject torpedo = Instantiate(TorpedoPrefab, this.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(_shootAnimationTime);
            // Restore the original rotation speed
            _rotationSpeed = _ogRotationSpeed;
            yield return new WaitForSeconds(_torPedoCoolDown);
        }
    }

    void LookAtCamera()
    {
        Vector3 direction = _camera.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, 0.01f);
    }
    void RotateAroundTarget(Transform target, float speed)
    {
        if (_healtPoints > 0) 
        {
            transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
            LookAtCamera();
        }
    }

    void MoveTowardsPlayer()
    {
        // Calculate the distance between the enemy and the player
        float distance = Vector3.Distance(transform.position, _camera.transform.position);

        // Check if the enemy is farther than the stopDistance
        if (distance > StopDistance)
        {
            // Move the enemy towards the player
            transform.position = Vector3.MoveTowards(transform.position, _camera.transform.position, 0.2f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile")){
            StartCoroutine(DamageBlink());
            Hited(other.gameObject.GetComponent<Projectile>());
        }

    }

    private void Hited(Projectile projectile)
    {
        _healtPoints -= projectile.Damage;
        if (_healtPoints <= 0)
        {
            StartCoroutine( Explote());
        }else
        {
            switch (projectile.Type)
            {
                case ProjectileType.Fireball:
                    _frozen = false;
                    _burned = true;
                    break;
                case ProjectileType.Electro:
                    _electrocuted = true; 
                    break;
                case ProjectileType.Freeze:
                    _frozen = true;
                    _burned = false;
                    break;

                default:
                    break;

            }
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
            
    }

    private IEnumerator Explote()
    {
        DamageBlink();
        yield return new WaitForSeconds(0.2f);
        MainMesh.enabled = false;
        Explosion.Play();
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

    private IEnumerator DamageBlink()
    {
        DamagedGameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        DamagedGameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        DamagedGameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        DamagedGameObject.SetActive(false);
    }


}


