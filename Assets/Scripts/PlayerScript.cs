using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    
    private string _animatorBool_walk = "walk";
    private string _animatorTrig_punch = "punch01";
    private string _animatorTrig_kick = "kick01";
    
    private string _animationState_Walking = "Walking";
    private string _animationTag_Attacking = "Attack";

    private Animator _animator;
    private PlayerInput _playerInput;
    private SpriteRenderer _renderer;

    private bool isWalk;

    private Vector2 walkDirection;
    public Vector2 WalkSpeed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        Debug.Log( "State "  + _animator.GetCurrentAnimatorStateInfo(0) );
        if ( _animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            
        }
        else {
            transform.Translate( walkDirection.x * WalkSpeed.x * Time.deltaTime, 0, walkDirection.y * WalkSpeed.y * Time.deltaTime );
        }
    }

    public void walk( InputAction.CallbackContext context )
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
        }
        if( walkDirection.x > 0 ){
            _renderer.flipX = false;
        }

    }

    public void punch( InputAction.CallbackContext context )
    {

         if ( context.performed ){
            Debug.Log( "Boom!" );
            _animator.SetTrigger( _animatorTrig_punch );

        }

    }

    public void kick( InputAction.CallbackContext context )
    {

         if ( context.performed ){
            Debug.Log( "Boom!" );
            _animator.SetTrigger( _animatorTrig_kick );

        }

    }
}
