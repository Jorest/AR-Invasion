using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileType _Type;

    [SerializeField] private int _Damage=1;

    public int Damage { get => _Damage; set => _Damage = value; }
    public ProjectileType Type { get => _Type; set => _Type = value; }
}
public enum ProjectileType
{
    Basic = 0,
    Fireball = 1,
    Electro = 2,
    Freeze = 3,
    Regenerating = 4,
    Invulnerable = 5,
}
