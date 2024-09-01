using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] List<GameObject> ProjectileTypes = new List<GameObject>();
    private int _projectileType = 0;



    public void ShotProjectile()
    {
        GameObject bullet = Instantiate(ProjectileTypes[_projectileType],transform.position,transform.rotation);
        Debug.Log(ProjectileTypes[_projectileType].name);
        _projectileType++;
        if (_projectileType >= ProjectileTypes.Count)
            _projectileType = 0;
        
        /*
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.rotation = this.transform.rotation;
        bullet.transform.position = this.transform.position;
        rb.AddForce(this.transform.forward * 1200f);
        Destroy(bullet, 3);
        */

    }




}
