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
    private float _cooldown = 1.5f;
    private bool _canShoot = true;
    private int _projectileType = 0;
    private int _projectileDamage = 3;

    private SoundManager _soundManager;
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
        _soundManager = SoundManager.Instance;
        _gameManager = GameManager.Instance;
        ShootButton.onClick.AddListener(Shoot);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Torpedo"))
        {
            int damage = col.GetComponent<Torpedo>().Damage;
            Destroy(col.gameObject);
            TakeDamage(damage);
        }
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
            _soundManager.PlaySound("Shoot");
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
        if (_health <= 0) {

            TextLifeNumber.text = ("0" + "/" + _healthTotal);

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
        _projectileDamage = (int)Mathf.Ceil(_projectileDamage * 1.25f); 
    }

    public void HandleTrigger(Collider other)
    {
        Debug.Log("Handling trigger with: " + other.gameObject.name);
    }



    #region UpgradeButtonMethods
    public void Heal()
    {
        HealAmount(10);
        _gameManager.StartNextLevel();
    }
    public void Fire()
    {

        _projectileType = 3;
        _gameManager.StartNextLevel();

    }
    public void Ice()
    {
        _projectileType = 2;
        _gameManager.StartNextLevel();
    }

    public void Electric()
    {
        _projectileType = 1;
        _gameManager.StartNextLevel();
    }

    public void Damage()
    {
        IncreaseDamage();
        _gameManager.StartNextLevel();
    }

    public void CoolDown()
    {

        _cooldown = _cooldown * 0.75f;
        _gameManager.StartNextLevel(); 



    }
    public void Health() {

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
