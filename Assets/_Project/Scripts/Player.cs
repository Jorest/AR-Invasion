using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    [SerializeField] List<GameObject> ProjectileTypes = new List<GameObject>();

    [Header("Life UI")]

    [SerializeField] Image LifeBar;
    private int _health = 10;
    
    private int _projectileType = 0;

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
    public void ShotProjectile()
    {
        GameObject bullet = Instantiate(ProjectileTypes[_projectileType],transform.position,transform.rotation);
 
        
        /*
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.rotation = this.transform.rotation;
        bullet.transform.position = this.transform.position;
        rb.AddForce(this.transform.forward * 1200f);
        Destroy(bullet, 3);
        */

    }

    public void TakeDamage(int damage)
    {
        
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
