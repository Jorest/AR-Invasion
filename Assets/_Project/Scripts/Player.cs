using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.
using TMPro;


public class Player : MonoBehaviour
{
    [SerializeField] List<GameObject> ProjectileTypes = new List<GameObject>();


    private int _health = 25;
    private int _healthTotal = 25;
    private float _cooldown = 1f;
    private bool _canShoot = true;
    private int _projectileType = 2;
    private int _projectileDamage = 1;

    private GameManager _gameManager;

    [Header("HUD elements")]
    [SerializeField] Button ShootButton;
    [SerializeField] Image LifeBar;
    [SerializeField] Image CoolDownCircle;
    [SerializeField] TextMeshProUGUI TextLifeNumber;

    [Header("Other")]
    [SerializeField] Transform ProjectilePos;

    public static Player Instance { get; private set; }
    public int ProjectileType { get => _projectileType; set => _projectileType = value; }

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

    private void Start()
    {
        _gameManager = GameManager.Instance;
        ShootButton.onClick.AddListener(Shoot);
    }
    public void Shoot()
    {

        StartCoroutine(IEShoot());        
    }
    private IEnumerator IEShoot()
    {
        if (_canShoot)
        {
            _canShoot = false;
            GameObject bullet = Instantiate(ProjectileTypes[_projectileType], ProjectilePos.position, ProjectilePos.rotation);
            bullet.GetComponent<Projectile>().Damage = _projectileDamage;
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
        if (_health > 0)
        {
            _health -= damage;
            LifeBar.fillAmount = ((float)_health / (float)_healthTotal);
            TextLifeNumber.text = (_health + "/" + _healthTotal);
        }
        if (_health == 0) {

            _gameManager.GameOver();

        }

    }

    public void HealAmount(int heal)
    {
        _health += heal;
        if (_health > _healthTotal)
        {
            _health = _healthTotal;

        }
        LifeBar.fillAmount = ((float)_health / (float)_healthTotal);
        TextLifeNumber.text = (_health + "/" + _healthTotal);
    }

    public void IncreaseDamage()
    {
        _projectileDamage = (int)Mathf.Ceil(_projectileDamage * 1.5f); 
    }

    public void HandleTrigger(Collider other)
    {
        Debug.Log("Handling trigger with: " + other.gameObject.name);
    }



    #region UpgradeButtonMethods
    public void Heal()
    {

        Debug.LogWarning("Healed Upgrade");
        HealAmount(15);
        _gameManager.StartNextLevel();
    }
    public void Fire()
    {

        _projectileType = 3;
        Debug.LogWarning("Fire Upgrade");
        _gameManager.StartNextLevel();

    }
    public void Ice()
    {

        _projectileType = 2;
        Debug.LogWarning("Ice Upgrade");
        _gameManager.StartNextLevel();
    }

    public void Electric()
    {
        _projectileType = 1;
        Debug.LogWarning("Electric Upgrade");
        _gameManager.StartNextLevel();
    }

    public void Damage()
    {

        IncreaseDamage();
        Debug.LogWarning("Damage Upgrade");
        _gameManager.StartNextLevel();
    }

    public void CoolDown()
    {

        _cooldown = _cooldown / 2;
        Debug.LogWarning("CoolDown Upgrade");
        _gameManager.StartNextLevel(); 



    }
    public void Health() {

        Debug.LogWarning("Health Upgrade");
        _healthTotal += 10;
        //upgade UI
        LifeBar.fillAmount = ((float)_health / (float)_healthTotal);
        TextLifeNumber.text = (_health + "/" + _healthTotal);

        _gameManager.StartNextLevel();

    }
    #endregion

    /*
     *0:Nature
     *1:Electro
     *2:ice
     *3:fire
     */

}
