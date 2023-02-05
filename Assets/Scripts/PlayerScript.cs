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
    protected string _animationTag_absorb = "absorb";
    private string _animatorBool_absorb = "absorbing";
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
    public HealthBar healthBar;
    private int prevHp;

    private void Awake()
    {
        base.Awake();
        _playerInput = GetComponent<PlayerInput>();
        healthBar.SetMaxHealth(maxHp);
        prevHp = currentHp;
    }

    // Update is called once per frame
    void Update()
    {
        if ( _animator.GetCurrentAnimatorStateInfo(0).IsTag( _animationTag_Attacking ))
        {
            
        }
        else if ( _animator.GetCurrentAnimatorStateInfo(0).IsTag( _animationTag_absorb ))
        {
            
        }
        else {
            // transform.Translate( walkDirection.x * WalkSpeed.x * Time.deltaTime, 0, walkDirection.y * WalkSpeed.y * Time.deltaTime );
            Vector3 move = new Vector3( walkDirection.x * WalkSpeed.x, 0, walkDirection.y * WalkSpeed.y );
            _controller.Move( move * Time.deltaTime );
        }

        if ( currentHp <= 0 )
        {
            Destroy( GetComponent<PlayerInput>() );
        }

        if( prevHp != currentHp ) {
            healthBar.SetHealth(currentHp);
        }
        prevHp = currentHp;
    }

    public void setHealth(int health) {
        health = health < 0 ? 0 : health;
        health = health > maxHp ? maxHp : health;
        
        currentHp = health;
        healthBar.SetHealth(currentHp);
    }

    public int getHealth() {
        return currentHp;
    }

    public List<float> getEnemiesDistance() {
        // Get list of all active enemies distance from player (sorted)
        List<float> allDistances = new List<float>();
        foreach(GameObject it in GameObject.FindGameObjectsWithTag(EntityTagNames.enemyTag)) {
            if(it.activeSelf) {
                allDistances.Add(Vector3.Distance(it.transform.position, transform.position));
            }
        }
        allDistances.Sort((a, b) => a.CompareTo(b));
        return allDistances;
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

         if ( context.performed )
         {
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

    public void Absorb( InputAction.CallbackContext context )
    {
        if ( context.performed )
        {
            _animator.SetBool( _animatorBool_absorb, true );
        }

        if ( context.canceled ){
             _animator.SetBool( _animatorBool_absorb, false );
        }
    }

}
