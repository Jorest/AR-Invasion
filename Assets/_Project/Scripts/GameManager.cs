using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Globalization;

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

    [SerializeField] private float startWaveDelay = 2f;




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


    public void EndLevel()
    {
        HUD.enabled = false;
        _lastPortal.VisualsOff();
        UpgradesManager.ShowUpgrades(_lastPortal.transform);
    }

    public void StartNextLevel()
    {
        StartCoroutine(IEStartLevel());
    }

    private IEnumerator IEStartLevel()
    {
        yield return new WaitForSeconds(2);
        _lastPortal.VisualsOn();
        HUD.enabled = true;
        EnemyManager.StartWave();
    }

    private void StartGame()
    {
        Destroy(DeleteButton.gameObject);
        StartButton.gameObject.SetActive(false);
        if (_lastPortal != null)
        {

            DissablePlaneRegonition();

            Vector3 portalocalRot = _lastPortal.transform.localEulerAngles;

            Vector3 cameraUp = Camera.main.transform.up;
            Vector3 portalUp = _lastPortal.transform.right;

            float angle = Vector3.Angle(cameraUp, portalUp);
            float dot = Vector3.Dot(cameraUp, portalUp);

            Debug.Log("dot" + dot + " portalocalRotX:" + portalocalRot.x);

            if (dot > 0)  // Dot product > 0 means same direction
            {
                portalocalRot.x=(180f);  // Aligned, set X rotation to 0
            }
            else  // Dot product < 0 means opposite direction
            {
                portalocalRot.x = (0);  // Opposite, set X rotation to 180
            }


            Debug.Log("dot" + dot + " portalocalRotX:" + portalocalRot.x);

            _lastPortal.transform.localEulerAngles = portalocalRot;
            _lastPortal.LockIn();
            HUD.gameObject.SetActive(true);

            StartCoroutine(StartWave());

            Destroy(StartButton.gameObject);
        }



    }

    public void GameOver()
    {
        GameOverScreen.SetActive(true);
        WaveText.text = ("Wave Number: " + EnemyManager.WaveNumber);
    }
    private IEnumerator StartWave()
    {

        yield return new WaitForSeconds(startWaveDelay);

        EnemyManager.StartFirstWave(_lastPortal.transform);

    }

    #region Portal related
    public void SetPortalEnable(bool enable)
    {
        _enablePortalSpawn = enable;
      //  StartButton.gameObject.SetActive(!_enablePortalSpawn);
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

    public void Upgrade (string name)
    {
        switch (name)
        {
            case "Heal":
                PlayerManager.Heal();
                break;
            case "Fire":
                PlayerManager.Fire();
                break;
            case "Ice":
                PlayerManager.Ice();
                break;
            case "Damage":
                PlayerManager.Damage();
                break;
            case "Health":
                PlayerManager.Health();
                break;
            case "Cooldown":
                PlayerManager.CoolDown();
                break;
            case "Electric":
                PlayerManager.Electric();
                break;

            default:
                Debug.LogError("invalid uprade name: "+ name);
                break;
        }
    }


    


}



