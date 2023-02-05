using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitPart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage( int damage )
    {
        // if(_animator.GetCurrentAnimatorStateInfo(0).IsName("TakingDamage")) return;

        SendMessageUpwards("ParentTakeDamage", damage);
    }
}
