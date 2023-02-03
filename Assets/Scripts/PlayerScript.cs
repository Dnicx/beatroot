using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{

    private Animator _animator;
    private PlayerInput _playerInput;

    private bool isWalk;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
    }

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    // void Update()
    // {

    // }

    public void walk( InputAction.CallbackContext context )
    {
        Debug.Log( context );
        Debug.Log( "walk" );
        if ( context.performed ){
            Debug.Log( "yes walk" );
            _animator.SetBool( "walk", true );
        }

        if ( context.canceled ){
             _animator.SetBool( "walk", false );
        }
    }
}
