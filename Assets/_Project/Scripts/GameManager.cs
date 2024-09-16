using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{

    private bool _enablePortalSpawn = true;

    [SerializeField] private Button StartButton;
    [SerializeField] private GameObject DeleteButton;
    [SerializeField] private EnemySpawner EnemySpanwer;
    [SerializeField] private Canvas HUD;
    [SerializeField] private Player Player;
    [SerializeField] private ARPlaneManager PlaneManager;


    [SerializeField] private Button ShootButton;


    public static GameManager Instance { get; private set; }
    public bool EnablePortalSpawn { get => _enablePortalSpawn; }

    private Portal _lastPortal = null; 
    
    public void SetPortalEnable(bool enable)
    {
        _enablePortalSpawn = enable;  
        StartButton.gameObject.SetActive(! _enablePortalSpawn);
    }
    
    


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

     void OnEnable()
    {
        StartButton.onClick.AddListener(StartGame);

    }

    void OnDisable()
    {

        StartButton.onClick.RemoveListener(StartGame);

    }

    private void StartGame()
    {
        Destroy(DeleteButton.gameObject);
        StartButton.gameObject.SetActive(false);

        if (_lastPortal != null)
        {
            Vector3 portalocalRot = _lastPortal.transform.localEulerAngles;
            portalocalRot.x = 0;
            _lastPortal.transform.localRotation = Quaternion.Euler(portalocalRot);
            _lastPortal.LockIn();
            HUD.gameObject.SetActive(true);

            DissablePlaneRegonition();
            StartCoroutine(StartWave());

        }




    }

    private IEnumerator StartWave()
    {

        yield return new WaitForSeconds(1f);

        EnemySpanwer.SpawnAliens(_lastPortal.transform);
        ShootButton.onClick.AddListener(Shoot);
        ShootButton.gameObject.SetActive(true);

    }


    public void UpdatePortal(Portal portal)
    {
        _lastPortal = portal;

    }

    void Shoot()
    {
        Player.TryShooting();

    }

    void DissablePlaneRegonition()
    {

        // Disable plane detection
        PlaneManager.enabled = false;
        
        // Optionally, disable existing planes
        foreach (var plane in PlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        
    }


}



