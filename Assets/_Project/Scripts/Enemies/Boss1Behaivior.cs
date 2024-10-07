using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Behaivior : MonoBehaviour
{

    private bool _electrocuted = false;
    private bool _burned = false;
    private bool _frozen = false;
    private bool _alive = true;
    private EnemySpawner _enemySpawner;
    private SoundManager _soundManager;

    private float _minDistance = 0.2f; // Minimum distance between enemies to avoid collisions
    private float _areaRadius = 0.4f; // Radius of the area in front of the portal
    private Vector3 _targetLocalPosition; // Random position to move towards


    private Camera _camera;
    private float _speed = 0.1f; // Variable movement speed of enemies
    private float _rotationSpeed = 8f;
    private float _shootAnimationTime = 0.1f;
    private float _distanceFoward = 2f;
    private bool _fowardone = false;

    //debuff timers 
    private float _fireTimer = 0f;
    private float _freezeTimer = 0f;
    private float _electroTimer = 0f;
    private float _pingInterval = 0.5f;
    private float _stopDistance = 1f;

    [Header("Adjustable Values")]
    [SerializeField] private float _speedValue = 0.1f;
    [SerializeField] private float _healtPoints = 5;
    [SerializeField] private float _torpedoCoolDown = 5f;

    [Header("Projectiles")]
    [SerializeField] GameObject TorpedoPrefab;


    [Header("Prefab")]
    [SerializeField] Transform TorpedoParent;
    [SerializeField] ParticleSystem Explosion;
    [SerializeField] ParticleSystem ChargingBall;
    [SerializeField] Collider Collider;
    [SerializeField] MeshRenderer MainMesh;
    [SerializeField] AudioSource audioSource;



    [Header("Visual Overlays")]
    [SerializeField] MeshRenderer Damaged;
    [SerializeField] MeshRenderer Electro;
    [SerializeField] MeshRenderer Fire;
    [SerializeField] MeshRenderer Ice;
    [SerializeField] MeshRenderer Shield;

    private float yMovementRange = 0.1f;

    #region UnityDefault

    void Start()
    {
        _soundManager = SoundManager.Instance;
        _enemySpawner = EnemySpawner.Instance;
        _speed = _speedValue;
        //move outside of the portal
        StartCoroutine(MoveFoward());
        //start shooting torpedos;
        StartCoroutine(StartShooting());

        GetNewRandomPosition(); // Set initial _target position to move towards


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


    #region Movement

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
    private void MoveEnemy()
    {
        // Move towards the _target position
        Vector3 direction = (_targetLocalPosition - transform.localPosition).normalized;
        transform.localPosition += direction * _speed * Time.deltaTime;

        // Check if the enemy is close to the _target position and get a new one
        if (Vector3.Distance(transform.localPosition, _targetLocalPosition) < 0.1f)
        {
            GetNewRandomPosition();
        }
    }
    private void GetNewRandomPosition()
    {
        // Define a random position within the area in front of the portal
        float radiusx = Random.Range(-_areaRadius, _areaRadius);
        float radiusz = Random.Range(-_areaRadius, _areaRadius);
        float radiusy = Random.Range(0.01f, 0.3f);

        //  randomDirection    += transform.parent.position;
        // Make sure the new position is within the specified area
        _targetLocalPosition = new Vector3(radiusx, radiusy, radiusz);
    }
    private void AvoidOtherEnemies()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, _minDistance);

        foreach (Collider enemy in nearbyEnemies)
        {
            // Check if the collider has the same tag ("Enemy" in this case)
            if (enemy.CompareTag("Enemy") && enemy != this.GetComponent<Collider>())
            {
                // Get _direction away from the other enemy
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
        if (distance > _stopDistance)
        {
            // Move the enemy towards the player
            transform.position = Vector3.MoveTowards(transform.position, _camera.transform.position, 0.2f * Time.deltaTime);
        }
    }
    #endregion
    private IEnumerator StartShooting()
    {
        yield return new WaitUntil(() => _fowardone);

        while (_healtPoints > 0)
        {

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
                         //fires projectile if not electrocuted

            if (!_electrocuted)
            {
                ChargingBall.Play();
                yield return new WaitForSeconds(ChargingBall.main.duration/4);
                _soundManager.PlaySound("AlienShoot", audioSource);
                GameObject torpedo = Instantiate(TorpedoPrefab, TorpedoParent.position, Quaternion.identity);
                ChargingBall.Stop();
                _enemySpawner.Projectiles.Add(torpedo);
            }
            yield return new WaitForSeconds(_shootAnimationTime);
            // Restore the original rotation speed
            _speed = initialSpeed;
            yield return new WaitForSeconds(_torpedoCoolDown);
        }
    }

    private void Hited(Projectile projectile)
    {
        _healtPoints -= projectile.Damage;
        if (_healtPoints <= 0)
        {
            Die();
        }
        else
        {
            // this values should be added as a variable later
            switch (projectile.Type)
            {
                case ProjectileType.Fireball:
                    StartCoroutine(Burned(1.5f));
                    break;
                case ProjectileType.Electro:
                    StartCoroutine(Electrocuted(0.3f));
                    break;
                case ProjectileType.Freeze:
                    StartCoroutine(Freezed(2.5f));
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

    private void Die()
    {
        if (_alive == true)
            _enemySpawner.ReportEnemyDeath();
        _alive = false;
        gameObject.GetComponent<Collider>().enabled = false;
        _soundManager.PlaySound("Explote", audioSource);
        StartCoroutine(Explote());
    }

    #region Debufs
    private IEnumerator Burned(float time)
    {
        if (!_burned)
        {
            _fireTimer = time;
            _burned = true;
            Fire.enabled = true;
            float nextDamageTime = _pingInterval;
            while (_fireTimer > 0)
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
            _freezeTimer = time;
            _speed = _speed / 2;
            while (_freezeTimer > 0)
            {
                _freezeTimer -= Time.deltaTime;

                yield return null;

            }
            _speed = _speedValue;
            _frozen = false;
            Ice.enabled = false;

        }
        else
        {
            _speed = _speed / 2;
            _freezeTimer = time; //reset the time 
        }
    }

    private IEnumerator Electrocuted(float time)
    {
        if (!_electrocuted)
        {
            _electrocuted = true;
            Electro.enabled = true;
            _electroTimer = time;



            while (_electroTimer > 0)
            {

                _electroTimer -= Time.deltaTime;
                yield return null;

            }
            _electrocuted = false;
            Electro.enabled = false;

        }
        else
        {
            _electroTimer = time; //reset the time 
        }

    }
    #endregion

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
        Damaged.enabled = (true);
        yield return new WaitForSeconds(0.1f);
        Damaged.enabled = (false);
        yield return new WaitForSeconds(0.1f);
        Damaged.enabled = (true);
        yield return new WaitForSeconds(0.1f);
        Damaged.enabled = (false);
    }


    private IEnumerator ShakeObject(float shakeDuration)
    {
        float shakeIntensity = 0.003f;

        // How long the shake lasts

        // How fast the shaking happens
        float shakeFrequency = 15.0f;

        float startTime = Time.time; // Capture the start time

        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;

        while (Time.time - startTime < shakeDuration)
        {
            // Generate random offsets for both position and rotation
            Vector3 randomPositionOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity)
            );

            Vector3 randomRotationOffset = new Vector3(
                Random.Range(-shakeIntensity * 10, shakeIntensity * 10),
                Random.Range(-shakeIntensity * 10, shakeIntensity * 10),
                Random.Range(-shakeIntensity * 10, shakeIntensity * 10)
            );

            // Apply the random position and rotation offsets
            transform.localPosition = originalPosition + randomPositionOffset;
            transform.localRotation = Quaternion.Euler(originalRotation.eulerAngles + randomRotationOffset);

            // Wait according to shake frequency (time between each shake)
            yield return new WaitForSeconds(1.0f / shakeFrequency);
        }

        // Restore original position and rotation after the shake
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

    #endregion

}
