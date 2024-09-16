using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class Player : MonoBehaviour
{

    [SerializeField] List<GameObject> ProjectileTypes = new List<GameObject>();


    private int _health = 10;
    private int _healthTotal = 10;
    private float _cooldown = 1f;
    private bool _canShoot = true;
    private int _projectileType = 0;


    [Header("HUD elements")]
    [SerializeField] Image LifeBar;
    [SerializeField] Image CoolDownCircle;

    public static Player Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void TryShooting()
    {

        StartCoroutine(Shoot());
        
        /*
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.rotation = this.transform.rotation;
        bullet.transform.position = this.transform.position;
        rb.AddForce(this.transform.forward * 1200f);
        Destroy(bullet, 3);
        */

    }




    private IEnumerator Shoot()
    {
        if (_canShoot)
        {
            _canShoot = false;
            GameObject bullet = Instantiate(ProjectileTypes[_projectileType], transform.position, transform.rotation);
            float elapsedTime = 0f;
            while (elapsedTime < _cooldown)
            {
                elapsedTime += Time.deltaTime;

                // Update the cooldown image fill (0 means no fill, 1 means full fill)
                CoolDownCircle.fillAmount =  (elapsedTime / _cooldown);

                yield return null;  // Wait for the next frame
            }


            _canShoot = true;

        }
        yield return null;

    }


    public void TakeDamage(int damage)
    {
        _health -= damage;
        LifeBar.fillAmount = (_health / _healthTotal);

    }

    public void HandleTrigger(Collider other)
    {
        Debug.Log("Handling trigger with: " + other.gameObject.name);
    }

    /*
     *0:Nature
     *1:Electro
     *2:ice
     *3:fire
     */

}
