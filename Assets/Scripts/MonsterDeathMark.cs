using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeathMark : MonoBehaviour
{
    int damage = 1;
    
    private float mdAttackTime;
    private float mdAttackTick;

    void Start()
    {
        mdAttackTime = 0f;
        mdAttackTick = 1f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponentInParent<Player>();
            if (player == null) return;
            
            if (Time.time >= mdAttackTime)
            {
                player.TakeDamage(damage);
                mdAttackTime = Time.time + mdAttackTick;
            }
        }
    }

    public void DestroyMark()
    {
        Destroy(gameObject);
    }
}
