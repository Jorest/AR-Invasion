using UnityEngine;
using UnityEngine.Events;

public class CollisionAndTriggerEvents : MonoBehaviour
{
    // UnityEvents without parameters (can be used in the Inspector)
    public UnityEvent onCollisionEnterEvent;
    public UnityEvent onTriggerEnterEvent;

    private Player _player;

    private void Start()
    {
        _player = Player.Instance;
    }


    // Called when a collision happens
    private void OnTriggerEnter(Collider col)
    {
        
        if (col.tag.Equals("Torpedo"))
        {
            int damage = col.GetComponent<Torpedo>().Damage;
            Destroy(col.gameObject);
            _player.TakeDamage(damage);
        }


    }



}
