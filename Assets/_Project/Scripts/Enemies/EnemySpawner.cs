using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject Alien1Prefab1;

    private int waveNumber = 1;
    private float spawnSpeed = 3;

    public int WaveNumber { get => waveNumber; set => waveNumber = value; }

    public void SpawnAliens(Transform portalTranform)
    {

        StartCoroutine(SpawnAliensType1(portalTranform));

    }

    private IEnumerator SpawnAliensType1(Transform portalTranform)
    {
        for (int i = 0; i <8; i++)
        {

            GameObject enemy = Instantiate(Alien1Prefab1, portalTranform.position, Quaternion.identity, portalTranform);
            //enable to scale ship to portal size
            //enemy.transform.localScale = enemy.transform.localScale * trans.localScale.x;
            enemy.transform.localPosition = Vector3.zero;
            enemy.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 90));
            yield return new WaitForSeconds (spawnSpeed);
        }
    }


}
