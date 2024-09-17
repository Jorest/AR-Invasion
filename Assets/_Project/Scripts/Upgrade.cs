using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "ScriptableObjects/Upgrade", order = 1)]
public class Upgrade : ScriptableObject
{
    public string UpgradeName;
    [TextArea(3, 4)] 
    public string Description;
    public bool infinite = true;
    public ProjectileType Type;



    // You can add more fields as needed
}


