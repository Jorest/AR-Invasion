using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "ScriptableObjects/Upgrade", order = 1)]
public class Upgrade : ScriptableObject
{
    public string upgradeName;
    public string description;
    public float value;  // This could be the amount the upgrade affects (e.g., +10% damage)
    public UpgradeType type; // e.g., Health, Damage, Speed, etc.

    // You can add more fields as needed
}

public enum UpgradeType
{
    Health,
    Damage,
    Speed
}
