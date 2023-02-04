using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : CharacterScript
{
    
    // 
    //  Declare
    // 
    private string _animatorTrig_punch = "punch01";
    private string _animatorTrig_kick = "kick01";
    private string _enemyTag = "Enemy";

    // 
    //  Public variable
    // 

    // 
    //  Private variable
    // 
    
    // Component
    private PlayerInput _playerInput;

    // state

    // variable
    public AudioClip punchSfx;
    public AudioClip kickSfx;


    private void Awake()
    {
        base.Awake();
        _playerInput = GetComponent<PlayerInput>();
    }

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
            _animator.SetTrigger( _animatorTrig_punch );

            bool isHit;
            isHit = Attack( CalAttackPos(), radius, 1, _enemyTag );

            if ( isHit )
            {
                _audioSrc.clip = punchSfx;
                _audioSrc.Play();
            }
            
        }

    }

    public void Kick( InputAction.CallbackContext context )
    {

         if ( context.performed ){
            _animator.SetTrigger( _animatorTrig_kick );

            bool isHit;
            isHit = Attack( CalAttackPos(), radius, 1, _enemyTag );

            if ( isHit )
            {
                _audioSrc.clip = kickSfx;
                _audioSrc.Play();
            }
            
        }

    }

}
