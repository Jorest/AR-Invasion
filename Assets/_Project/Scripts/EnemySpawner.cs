using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject Alien1Prefab1;
    public void SpawnAlien(Transform trans)
    {
        GameObject enemy= Instantiate(Alien1Prefab1, trans.position, Quaternion.identity, trans );
        //enable to scale ship to portal size
        //enemy.transform.localScale = enemy.transform.localScale * trans.localScale.x;
        enemy.transform.localPosition = Vector3.zero;
        enemy.transform.localRotation = Quaternion.Euler(new Vector3 (-90,0,90));
    }


}
