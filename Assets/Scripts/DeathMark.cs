using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMark : MonoBehaviour
{
    int damage = 1;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.TakeDamage(damage);
        }
    }

    public void DestroyMark()
    {
        Destroy(gameObject);
    }
}
