using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    
    // 
    //  Animator state
    // 
    private string _animatorBool_walk = "walk";
    private string _animatorTrig_punch = "punch01";
    private string _animatorTrig_kick = "kick01";
    
    private string _animationState_Walking = "Walking";
    private string _animationTag_Attacking = "Attack";
    
    private string _damage_message = "TakeDamage";
    private string _enemyTag = "Enemy";

    // 
    //  Public variable
    // 
    public Vector2 WalkSpeed;
    public Vector3 attackPos_relative;
    public float radius;

    // [Tooltip("fall speed in Y axis ( positive is pulling down )")]
    // public float gravity;


    // 
    //  Private variable
    // 
    
    // Component
    private Animator _animator;
    private PlayerInput _playerInput;
    private SpriteRenderer _renderer;
    private CharacterController _controller;

    // state
    private Vector2 walkDirection;
    private bool isWalk;

    // variable
    private int _facing = 1; // 1 = facing right, -1 = facing left

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _renderer = GetComponent<SpriteRenderer>();
        _controller = GetComponent<CharacterController>();
    }

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if ( _animator.GetCurrentAnimatorStateInfo(0).IsTag( _animationTag_Attacking ))
        {
            
        }
        else {
            // transform.Translate( walkDirection.x * WalkSpeed.x * Time.deltaTime, 0, walkDirection.y * WalkSpeed.y * Time.deltaTime );
            Vector3 move = new Vector3( walkDirection.x * WalkSpeed.x, 0, walkDirection.y * WalkSpeed.y );
            _controller.Move( move * Time.deltaTime );
        }
    }

    public void Walk( InputAction.CallbackContext context )
    {
        Debug.Log( context.ReadValue<Vector2>() );
        Debug.Log( "walk" );
        if ( context.performed ){
            Debug.Log( "yes walk" );
            _animator.SetBool( _animatorBool_walk, true );

        }

        if ( context.canceled ){
             _animator.SetBool( _animatorBool_walk, false );
        }

        walkDirection = context.ReadValue<Vector2>();
        if( walkDirection.x < 0 ){
            _renderer.flipX = true;
            _facing = -1;
        }
        if( walkDirection.x > 0 ){
            _renderer.flipX = false;
            _facing = 1;
        }

    }

    public void Punch( InputAction.CallbackContext context )
    {

         if ( context.performed ){
            Debug.Log( "Boom!" );
            _animator.SetTrigger( _animatorTrig_punch );

            // Vector3 globalAttackPos = CalAttackPos();
            Attack( CalAttackPos(), radius, 1 );

        }

    }

    public void Kick( InputAction.CallbackContext context )
    {

         if ( context.performed ){
            Debug.Log( "Boom!" );
            _animator.SetTrigger( _animatorTrig_kick );

            // Vector3 globalAttackPos = CalAttackPos();
            Attack( CalAttackPos(), radius, 1 );


        }

    }

    private Vector3 CalAttackPos()
    {
        Vector3 globalAttackPos = transform.position;
        globalAttackPos += Vector3.Scale( attackPos_relative,  new Vector3( _facing, 1, 1 ) );

        return globalAttackPos;
    }

    private void Attack( Vector3 attackPos, float attackRadius, int power )
    {
        int maxColider = 10;
        Collider[] _hitCollider = new Collider[ maxColider ];
        int _numfound = Physics.OverlapSphereNonAlloc( attackPos, attackRadius, _hitCollider );

        for( int i = 0; i < _numfound ; i++ )
        {
            if (_hitCollider[i].gameObject.tag == _enemyTag)
            {
                _hitCollider[i].SendMessage( _damage_message, power );
            }
            
        }


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
