using UnityEngine;
using UnityEngine.Events;

public class TriggerColOnPlayer : MonoBehaviour
{
    private Player _player;

    private void Start()
    {
        _player = Player.Instance;
    }


    // Called when a collision happens
    private void OnTriggerEnter(Collider col)
    {
        Debug.LogWarning("``1111");

        if (col.tag.Equals("Torpedo"))
        {
            Debug.LogWarning("DWDWFEWQGFRGF");
            int damage = col.GetComponent<Torpedo>().Damage;
            Destroy(col.gameObject);
            _player.TakeDamage(damage);
        }


    }



}
