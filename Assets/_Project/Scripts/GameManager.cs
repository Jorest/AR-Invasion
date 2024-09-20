using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    private bool _enablePortalSpawn = true;

    [Header("Setting AR")]

    [SerializeField] private Button StartButton;
    [SerializeField] private GameObject DeleteButton;
    [SerializeField] private ARSession ArSesh;



    [Header("Managers/Controllers")]
    [SerializeField] private EnemySpawner EnemyManager;
    [SerializeField] private Player PlayerManager;
    [SerializeField] private ARPlaneManager PlaneManager;
    [SerializeField] private UpgradeManager UpgradesManager;

    [Header("UI")]

    [SerializeField] private Canvas HUD;
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private TextMeshProUGUI WaveText;






    public static GameManager Instance { get; private set; }
    public bool EnablePortalSpawn { get => _enablePortalSpawn; }

    private Portal _lastPortal = null; 
    
 

    #region Unity Default
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
    #endregion


    public void WaveEnded(int waveNum)
    {
        HUD.enabled = false;
        UpgradesManager.ShowUpgrades(_lastPortal.transform);
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

    public void GameOver()
    {
        GameOverScreen.SetActive(true);
        WaveText.text = ("Wave Number: " + EnemyManager.WaveNumber);
    }
    private IEnumerator StartWave()
    {

        yield return new WaitForSeconds(1f);

        EnemyManager.StartGame(_lastPortal.transform);

    }

    #region Portal related
    public void SetPortalEnable(bool enable)
    {
        _enablePortalSpawn = enable;
        StartButton.gameObject.SetActive(!_enablePortalSpawn);
    }

    public void UpdatePortal(Portal portal)
    {
        _lastPortal = portal;

    }

    #endregion

 

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

    private void EnablePlaneRegonition()
    {

        // Disable plane detection
        PlaneManager.enabled = true;

        // Optionally, disable existing planes
        foreach (var plane in PlaneManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }

    }


    public void RestartGame()
    {

        EnablePlaneRegonition();
        StartCoroutine(ResetARSession());

       

    }

    private IEnumerator<WaitForSeconds> ResetARSession()
    {
        PlaneManager.enabled = true;

        // Disable the AR session
        ArSesh.Reset();

        // Wait for one frame to let the session reset
        yield return new WaitForSeconds(0.1f);

        // Get the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);

        // Optionally, restart the AR session if needed
        // You can re-enable AR features here
    }




    #region UpgradeButtonMethods
    public void Heal() { Debug.LogWarning("1"); }
    public void Fire() { Debug.LogWarning("2"); }
    public void Ice() { Debug.LogWarning("3"); }
    public void CoolDown() { Debug.LogWarning("4"); }
    public void Damage() { Debug.LogWarning("5"); }
    public void Health() { Debug.LogWarning("6"); }
    public void Electric() { Debug.LogWarning("7"); }
    #endregion


}



