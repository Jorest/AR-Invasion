using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> EnemiesPrefabs;


    private int _waveNumber = 1;
    private int _packAmount = 1;
    private float spawnDelay = 5;
    private int _enemyCount = 0;
    private Transform _portalTransform = null;
    private List<GameObject> _projectiles = new List<GameObject>();
    [SerializeField] GameManager GameManager;
    public static EnemySpawner Instance { get; private set; }

    public int WaveNumber { get => _waveNumber; set => _waveNumber = value; }
    public List<GameObject> Projectiles { get => _projectiles; set => _projectiles = value; }

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

    public void StartFirstWave(Transform portalTransform)
    {
        if (_portalTransform == null)
            _portalTransform = portalTransform;

       // _portalTransform.position = portalTransform.position;
       // _portalTransform.rotation = portalTransform.rotation;

        StartCoroutine(IEStarWave(_waveNumber));
        _waveNumber++;
    }
    public void StartWave()
    {
        if (_portalTransform == null)
            Debug.LogError("no posiiton for spawning");
        
        if (_waveNumber ==(4|7|10) )
        {
            //Star boss
            _waveNumber++;

        }
        else
        {
            StartCoroutine(IEStarWave(_waveNumber));
            _waveNumber++;
        }
      
    }

    private IEnumerator IEStarWave(int waveNumber)
    {
        _enemyCount = waveNumber * _packAmount;
        for (int i = 0; i < waveNumber; i++)
        {
            SpawnRandomEnemies();
            yield return new WaitForEndOfFrame();
        }
    }
    public void KillProjectiles()
    {
        foreach (GameObject projectile in _projectiles)
        {
            Destroy(projectile);
        }
    }



    public void SpawnRandomEnemies()
    {
        int randomIndex = Random.Range(0, 3); // 0 - 2
        StartCoroutine(SpawnAliensType(randomIndex));
     
    }
    public void ReportEnemyDeath()
    {
        _enemyCount--;
        if (_enemyCount <= 0)
        {
            Debug.LogWarning("LEVEL END :D");
            GameManager.EndLevel();
        }
    }

    private IEnumerator SpawnAliensType(int alienType)
    {
        for (int i = 0; i <_packAmount; i++)
        {

            GameObject enemy = Instantiate(EnemiesPrefabs[alienType], _portalTransform.position, Quaternion.identity, _portalTransform);
            //enable to scale ship to portal size
            //enemy.transform.localScale = enemy.transform.localScale * trans.localScale.x;
            enemy.transform.localPosition = Vector3.zero;
            enemy.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 90));
            yield return new WaitForSeconds (spawnDelay);
        }
    }

  
    




}
