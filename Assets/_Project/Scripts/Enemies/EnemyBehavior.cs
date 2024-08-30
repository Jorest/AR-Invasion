using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
  
    private bool _electrocuted = false;
    private bool _burned = false;
    private bool _frozen = false;
    private int _healtPoints = 4;


    private Camera _camera;
    private float _speed = 0.5f; // Movement speed
    private float _distanceFoward =2f;
    private bool _fowardone = false;
    [SerializeField] private float StopDistance = 1f;
    [SerializeField] GameObject DamagedGameObject;

  


    private float yMovementRange = 0.1f;

    void Start()
    {
        StartCoroutine(MoveFoward());
    }

    private void OnEnable()
    {
        _camera = Camera.main;
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

    void LookAtCamera()
    {
        Vector3 direction = _camera.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, 0.01f);
    }


    void Update()
    {
        if (_fowardone)
        {
            RotateAroundTarget(_camera.transform, 10);
            MoveTowardsPlayer();
        }
    }

    void RotateAroundTarget(Transform target, float speed)
    {
        // Rotate the enemy around the target (player) on the Y axis
        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
        LookAtCamera();
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
        Debug.LogWarning("heyooo");
        StartCoroutine(DamageBlink());
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


