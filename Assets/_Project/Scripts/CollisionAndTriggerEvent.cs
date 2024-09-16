using UnityEngine;

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
        if (col.tag.Equals("Torpedo"))
        {
            int damage = col.GetComponent<Torpedo>().Damage;
            Destroy(col.gameObject);
            _player.TakeDamage(damage);
        }


    }



}
