using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{

    private Animator _animator;

    private string _takeDamage_trigger = "takeDamage";
    private string _punch_trigger = "punch01";

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage( int damage )
    {
        currentHp -= damage;
        _animator.SetTrigger( _takeDamage_trigger );
    }

    public void Attack()
    {
        // TODO: check player hitbox
        
        _animator.SetTrigger( _punch_trigger );

    }
}
