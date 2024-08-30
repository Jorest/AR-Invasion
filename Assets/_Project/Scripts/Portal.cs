using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

public class Portal : MonoBehaviour
{
    private bool _reported = false;
    private GameManager _gameStarter;
    [SerializeField] XRInteractableAffordanceStateProvider _xrAffordanceProv;
    [SerializeField] XRGrabInteractable _xrInteractable; 
    private void OnEnable()
    {
        _gameStarter = GameManager.Instance;
        _reported = true;
        _gameStarter.UpdatePortal(this);
    }

    public void LockIn()
    {
       // _xrInteractable.StopAllCoroutines();
        _xrInteractable.enabled = false;
    }

}
