using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    private bool _electrocuted = false;
    private bool _burned = false;
    private bool _frozen = false;
    private bool alive = true;
    private EnemySpawner _enemySpawner;

    private float _healtPoints = 5;
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

    private float _fireTimer = 0f;
    private float _freezeTime = 0f;
    private float _electroTime = 0f;
    private float _pingInterval = 0.3f;

    [SerializeField] private float StopDistance = 1f;
    [SerializeField] private float SpeedValue =  0.1f;
    [SerializeField] GameObject DamagedGameObject;
    [SerializeField] ParticleSystem Explosion;
    [SerializeField] Collider Collider;
    [SerializeField] MeshRenderer MainMesh;
    [SerializeField] GameObject TorpedoPrefab;


    [Header("Visual Overlays")]
    [SerializeField] MeshRenderer Elctro;
    [SerializeField] MeshRenderer Fire;
    [SerializeField] MeshRenderer Ice;
    [SerializeField] MeshRenderer Shield;









    private float yMovementRange = 0.1f;

    #region UnityDefault

    void Start()
    {
        _enemySpawner = EnemySpawner.Instance;
        _speed = SpeedValue;
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
        Shield.enabled = true;
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
        Collider.enabled = true;
        Shield.enabled = false;
        _fowardone = true;
        // transform.position = new Vector3(0,0,0);
    }       
    private IEnumerator StartShooting()
    {
        yield return new WaitUntil(() => _fowardone);

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
            _enemySpawner.Projectiles.Add(torpedo);
   
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
            Die();
        }else
        {
            switch (projectile.Type)
            {
                case ProjectileType.Fireball:
                     StartCoroutine(Burned(3f));
                    break;
                case ProjectileType.Electro:
                    _electrocuted = true; 
                    break;
                case ProjectileType.Freeze:
                    StartCoroutine(Freezed(3f));
                    break;

                default:
                    break;

            }
        }
    }

    private void Hited(float damage)
    {
        _healtPoints -= damage;
        if (_healtPoints <= 0)
        {
            Die();
        }
    }


    private IEnumerator Burned(float time)
    {
        if (!_burned)
        {
            _fireTimer = time;
            _burned = true;
            Fire.enabled = true;
            float nextDamageTime = _pingInterval;
            while (_fireTimer > 0 )             
            {
                _fireTimer -= Time.deltaTime;
                nextDamageTime -= Time.deltaTime;

                if (nextDamageTime <= 0)
                {
                    Fire.enabled = !Fire.enabled;
                    Hited(0.5f);
                    nextDamageTime = _pingInterval; // Reset the timer for the next damage
                }

                yield return null;

            }
            _burned = false;
            Fire.enabled = false;

        }
        else
        {
            _fireTimer = time; //reset the time 
        }

    }

    private IEnumerator Freezed(float time)
    {
        if (!_frozen)
        {
            _frozen = true;
            Ice.enabled = true;
            _freezeTime = time;
            _speed = _speed / 2;
            while (_freezeTime > 0)
            {
                _freezeTime -= Time.deltaTime;
                yield return null;

            }
            _speed = SpeedValue;
            _frozen = false;
            Ice.enabled = false;

        }
        else
        {
            _speed =_speed / 2;
            _fireTimer = time; //reset the time 
        }

    }



    private IEnumerator Electrocuted()
    {
        yield return null;
    }

    private void Die()
    {
        if (alive == true) 
            _enemySpawner.ReportEnemyDeath();
        alive = false;
        gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(Explote());
    }

    #region Visual Methods

    private IEnumerator Explote()
    {
        StopCoroutine(DamageBlink());
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


