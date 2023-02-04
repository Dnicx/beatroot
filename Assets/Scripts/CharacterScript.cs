using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : Entity
{
    // What is a character? A character can walk around, attack get hurt

    // 
    // Declaration
    // 

    protected string _animatorBool_walk = "walk";
    
    protected string _animationState_Walking = "Walking";
    protected string _animationTag_Attacking = "Attack";
    
    protected string _damage_message = "TakeDamage";
    private string _takeDamage_trigger = "takeDamage";

    // 
    //  Public variable
    // 
    public Vector2 WalkSpeed;
    public Vector3 attackPos_relative;
    public float radius;
    
    // 
    //   variable
    // 
    
    // Component
    protected Animator _animator;
    protected SpriteRenderer _renderer;
    protected CharacterController _controller;
    protected AudioSource _audioSrc;

    // TODO: add sound player

    // state
    protected Vector2 walkDirection;
    protected bool isWalk;

    protected int _facing = 1; // 1 = facing right, -1 = facing left

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _controller = GetComponent<CharacterController>();
        _audioSrc = GetComponent<AudioSource>();
    }

    protected Vector3 CalAttackPos()
    {
        Vector3 globalAttackPos = transform.position;
        globalAttackPos += Vector3.Scale( attackPos_relative,  new Vector3( _facing, 1, 1 ) );

        return globalAttackPos;
    }

    protected bool Attack( Vector3 attackPos, float attackRadius, int power, string doDamageToTag )
    {
        int maxColider = 10;
        Collider[] _hitCollider = new Collider[ maxColider ];
        int _numfound = Physics.OverlapSphereNonAlloc( attackPos, attackRadius, _hitCollider );

        int hitCounter = 0;
        for( int i = 0; i < _numfound ; i++ )
        {
            if (_hitCollider[i].gameObject.tag == doDamageToTag)
            {
                hitCounter += 1;
                _hitCollider[i].SendMessage( _damage_message, power );
            }
            
        }

        return hitCounter > 0;

    }

    public void TakeDamage( int damage )
    {
        currentHp -= damage;
        _animator.SetTrigger( _takeDamage_trigger );
    }

    
# if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Vector3 globalAttackPos = CalAttackPos();

        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere( globalAttackPos, radius );
    }

# endif
}
