using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    private bool _electrocuted = false;
    private bool _burned = false;
    private bool _frozen = false;

    private int _healtPoints = 4;
    private float _torPedoCoolDown = 5f;
    private float minDistance = 0.2f; // Minimum distance between enemies to avoid collisions
    private float areaRadius = 0.4f; // Radius of the area in front of the portal
    public float changeTargetTime = 3f; // Time to change direction or position
    private Vector3 targetLocalPosition; // Random position to move towards


    private Camera _camera;
    private float _speed = 0.1f; // Movement speed of enemies
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
        //move outside of the portal
        StartCoroutine(MoveFoward());
        //start shooting torpedos;
        StartCoroutine(StartShooting());

        GetNewRandomPosition(); // Set initial target position
        

    }

    void Update()
    {
        if (_fowardone)
        {
            //RotateAroundTarget(_camera.transform, _rotationSpeed);
            // MoveTowardsPlayer();

            MoveEnemy();
     //      AvoidOtherEnemies();    
            LookAtCamera();
            // Ensure enemies don't collide with each other
            AvoidOtherEnemies();

        }
    }

    private void OnEnable()
    {
        _camera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            StartCoroutine(DamageBlink());
            Hited(other.gameObject.GetComponent<Projectile>());
        }

    }

    #endregion

    private void MoveEnemy()
    {
        // Move towards the target position
        Vector3 direction = (targetLocalPosition - transform.localPosition).normalized;
        transform.localPosition += direction * _speed * Time.deltaTime;

        // Check if the enemy is close to the target position and get a new one
        if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.1f)
        {
            GetNewRandomPosition();
        }
    }
    private void GetNewRandomPosition()
    {
        // Define a random position within the area in front of the portal
        float radiusx = Random.Range(-areaRadius, areaRadius );
        float radiusz = Random.Range(-areaRadius, areaRadius);
        float radiusy = Random.Range(0.01f, 0.3f);

        //  randomDirection    += transform.parent.position;
        // Make sure the new position is within the specified area
        targetLocalPosition = new Vector3(radiusx, radiusy, radiusz);
    }
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

        while (_healtPoints > 0 ) { 
        
            // Lerp the _rotationSpeed to 0 over time
            float elapsedTime = 0f;
            float duration = 0.5f; // Adjust this value to control the speed of the lerp
            float initialSpeed = _speed;


            // nested while to lerp the speed to 0 
            while (elapsedTime < duration)
            {
                _rotationSpeed = Mathf.Lerp(initialSpeed, 0f, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            _speed = 0f; // Ensure it is exactly 0 after the lerp
            GameObject torpedo = Instantiate(TorpedoPrefab, this.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(_shootAnimationTime);
            // Restore the original rotation speed
            _speed = initialSpeed;
            yield return new WaitForSeconds(_torPedoCoolDown);
        }
    }

    private void AvoidOtherEnemies()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, minDistance);

        foreach (Collider enemy in nearbyEnemies)
        {
            // Check if the collider has the same tag ("Enemy" in this case)
            if (enemy.CompareTag("Enemy") && enemy != this.GetComponent<Collider>())
            {
                // Get direction away from the other enemy
                Vector3 avoidanceDirection = transform.localPosition - enemy.transform.localPosition;
                avoidanceDirection.y = 0; // Keep on the same plane
                transform.localPosition += avoidanceDirection.normalized * _speed * Time.deltaTime;
            }
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

    #region Visual Methods
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

    #endregion
}


