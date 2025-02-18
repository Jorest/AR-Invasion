using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Reflection;

public class UpgradeManager : MonoBehaviour
{
    private bool _upgraded = false;
    public List<Upgrade> availableUpgrades;  // Populate this in the Inspector with all possible upgrades
    public int numberOfOptions = 3;  // How many options to present to the player
    public float UpgradeShowDelay = 2f;
    [SerializeField] GameManager gameManager;

    [Header("UI selection")]
    [SerializeField] GameObject UPgradeUI;

    [SerializeField] List<TextMeshProUGUI> Fields;
    [SerializeField] List<UnityEngine.UI.Image> FrameImages;
    [SerializeField] List<Transform> TransformsIcon;
    [SerializeField] List<Transform> TransformsUpgradeUI;
    [SerializeField] List<Button> ConfimButtons;



    public void ShowUpgrades(Transform portalTransform)
    {
        UPgradeUI.transform.position = portalTransform.position;

        Vector3 TowardsX = portalTransform.right;
        TowardsX.y = 0;
        TowardsX.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(TowardsX);

        Vector3 originalRot = UPgradeUI.transform.eulerAngles;

        UPgradeUI.transform.parent = portalTransform;
        //UPgradeUI.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 90));
        UPgradeUI.transform.localEulerAngles = new Vector3( 90f,180, -90f);
        //UPgradeUI.transform.eulerAngles= new Vector3 (originalRot.x, originalRot.y+ portalTransform.eulerAngles.y, originalRot.z);
        StartCoroutine(UpgradeSet());
    }

    private IEnumerator UpgradeSet()
    {
        yield return new WaitForSeconds(UpgradeShowDelay);
        UPgradeUI.SetActive(true);
        UpdateUIOpen();

        // Randomly select a set of upgrades to present to the player
        List<Upgrade> selectedUpgrades = GetRandomUpgrades();

        // Present them to the player (this would depend on your UI system)
        foreach (var upgrade in selectedUpgrades)
        {
            if (upgrade.infinite == false)
            {
                Debug.LogWarning("tryng to remove upgrade");
                availableUpgrades.Remove(upgrade);
            }
        }
    }

    List<Upgrade> GetRandomUpgrades()
    {
        List<Upgrade> randomUpgrades = new List<Upgrade>();
        // Shuffle and pick the desired number of random upgrades
        List<Upgrade> shuffledUpgrades = new List<Upgrade>(availableUpgrades);
        for (int i = 0; i < numberOfOptions; i++)
        {
            if (shuffledUpgrades.Count > 0)
            {
                int randomIndex = Random.Range(0, shuffledUpgrades.Count);
                randomUpgrades.Add(shuffledUpgrades[randomIndex]);
                //set all the visuals and text for the respective upgrade   
                Upgrade up = shuffledUpgrades[randomIndex];
                Fields[i].text = up.Description;
                FrameImages[i].color = up.FrameColor;
                Instantiate(up.VisualElement, TransformsIcon[i].position, TransformsIcon[i].rotation, TransformsIcon[i]);
                AssingUpgradeMethod(up.UpgradeName, ConfimButtons[i]);
                shuffledUpgrades.RemoveAt(randomIndex);  // Avoid selecting the same upgrade again
                         
            }
        }
        _upgraded = true;
        return randomUpgrades;
    }

    private void AssingUpgradeMethod(string name, Button button)
    {

        if (_upgraded)
        {
            Debug.LogWarning("Not the first upgrade");
            RemoveLastButtonListener(button);
        }

        button.onClick.AddListener(() => gameManager.Upgrade(name));

    }

    //referenced by Editor
    public void UpdateUIClose()
    {
        for (int i = 0; i < TransformsUpgradeUI.Count; i++)
        {
            TransformsUpgradeUI[i].gameObject.SetActive(false);
            Destroy(TransformsIcon[i].GetChild(0).gameObject);
        }
       
    }

    public void UpdateUIOpen()
    {
        for (int i = 0; i < TransformsUpgradeUI.Count; i++)
        {
            TransformsUpgradeUI[i].gameObject.SetActive(true);
        }


    }

    //from chat gpt 
    void RemoveLastButtonListener(Button button)
    {
        UnityEventBase unityEventBase = button.onClick;

        // Use reflection to access the private field "m_Calls" in UnityEventBase
        FieldInfo fieldInfo = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            // Get the object that holds the call list
            object callGroup = fieldInfo.GetValue(unityEventBase);

            // Access the runtime calls list ("m_RuntimeCalls")
            FieldInfo callsField = callGroup.GetType().GetField("m_RuntimeCalls", BindingFlags.NonPublic | BindingFlags.Instance);
            var calls = callsField.GetValue(callGroup) as System.Collections.IList;

            // Check if there are any listeners
            if (calls != null && calls.Count > 0)
            {
                // Remove the last listener
                calls.RemoveAt(calls.Count - 1);
                Debug.Log("Last listener removed");
            }
            else
            {
                Debug.Log("No listeners found");
            }
        }
    }

}
