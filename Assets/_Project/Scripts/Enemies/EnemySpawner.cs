using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> EnemiesPrefabs;

    [SerializeField] List<GameObject> BossesPrefabs;

    private int _waveNumber = 1;
    private int _packAmount = 2;
    private float _spawnPackDelay = 5;
    private float _spawnDelay = 2.5f;
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
        Debug.LogWarning("StartFirstWave");
        if (_portalTransform == null)
            _portalTransform = portalTransform;

        StartCoroutine(IEStarWave(_waveNumber));
        _waveNumber++;
    }
    public void StartWave()
    {
        if (_portalTransform == null) 
        { 
            Debug.LogError("no posiiton for spawning");
        }
        if (_waveNumber ==4 )
        {
            Debug.LogWarning("BOSS FIGHT 1");
            SpawnBoss(0); 
            _waveNumber++;
            _packAmount++;

        }
        else
        {
            StartCoroutine(IEStarWave(_waveNumber));
            _waveNumber++;
        }

    }

    private IEnumerator IEStarWave(int waveNumber)
    {
        Debug.LogWarning("IE StarWave WAVE NUMBER " + _waveNumber);
        _enemyCount = waveNumber * _packAmount;
        for (int i = 0; i < waveNumber; i++)
        {
            int randomIndex = Random.Range(0, 3); // 0 - 2
            StartCoroutine(SpawnAliensType(randomIndex));
            yield return new WaitForSeconds(_spawnPackDelay);
        }
    }
    public void KillProjectiles()
    {
        foreach (GameObject projectile in _projectiles)
        {
            if (projectile!=null)
            {
                Destroy(projectile);

            }
        }
        _projectiles.Clear();
    }

    public void ReportEnemyDeath()
    {
        _enemyCount--;
        if (_enemyCount <= 0)
        {
            KillProjectiles();
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
            yield return new WaitForSeconds (_spawnDelay);
        }
    }


    private void SpawnBoss(int bossNumber)
    {
       
            GameObject enemy = Instantiate(BossesPrefabs[bossNumber], _portalTransform.position, Quaternion.identity, _portalTransform);
            //enable to scale ship to portal size
            //enemy.transform.localScale = enemy.transform.localScale * trans.localScale.x;
            enemy.transform.localPosition = Vector3.zero;
            enemy.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 90));
       
    }




}
