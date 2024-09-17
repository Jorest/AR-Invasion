using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public List<Upgrade> availableUpgrades;  // Populate this in the Inspector with all possible upgrades
    public int numberOfOptions = 3;  // How many options to present to the player

    void EndLevel()
    {
        // Randomly select a set of upgrades to present to the player
        List<Upgrade> selectedUpgrades = GetRandomUpgrades();

        // Present them to the player (this would depend on your UI system)
        foreach (var upgrade in selectedUpgrades)
        {
            Debug.Log("Option: " + upgrade.upgradeName);
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
                shuffledUpgrades.RemoveAt(randomIndex);  // Avoid selecting the same upgrade again
            }
        }

        return randomUpgrades;
    }
}
