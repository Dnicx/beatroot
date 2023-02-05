using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : Entity
{

    public GameObject head;
    public GameObject pawR;
    public GameObject pawL;

    public GameObject headShadow;
    public GameObject pawRShadow;
    public GameObject pawLShadow;

    public Vector3 headWindingOffset;
    public Vector3 pawRWindingOffset;
    public Vector3 pawLWindingOffset;

    public float attackDelay = 1;

    public float windingSpeed = 1.0f;
    public float attackSpeed = 1.0f;
    public float speedScaler = 1.0f;
    public float cooldownSec = 2.0f;
    private bool isCooledDown = true;
    [SerializeField] private float attackProb = 40.0f;

    [SerializeField] private float deathDelaySec = 1.0f;
    [SerializeField] private Color32 damageColor;
    [SerializeField] private float colorFreq = 5.0f;
    [SerializeField] private int colorRound = 3;
    private Color32 startColor;
    private Color32 endColor;

    private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
    private Animator _animator;
    private CharacterController _controller;

    private string _AttackRight_trigger = "AttackingRight";
    private string _AttackLeft_trigger = "AttackingLeft";

    private GameObject _Player;

    private float offsetX = 0f;
    private float offsetZ = 0f;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        AttackLeft();
        print(" child cound" + _renderers.Count );
    }

    void Awake()
    {
        _renderers.Add( head.GetComponent<SpriteRenderer>() );
        _renderers.Add( pawR.GetComponent<SpriteRenderer>() );
        _renderers.Add( pawL.GetComponent<SpriteRenderer>() );

        _animator = head.GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _Player = GameManager.Instance.getPlayer();

    }

    // Update is called once per frame
    void Update()
    {
        castShadow( head, headShadow );
        castShadow( pawR, pawRShadow );
        castShadow( pawL, pawLShadow );

        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            FollowPlayer();

        if( isCooledDown )
        {
            float attackRand = Random.Range( 0, 100.0f );
            if ( attackRand < attackProb )
            {
                if ( attackRand < attackProb / 2 )
                    AttackLeft();
                else
                    AtackRight();
            }
        }
    }

    private void FollowPlayer()
    {
        Debug.Log( "_Player " + _Player.transform.position );

        if ( Random.Range( 0f, 1f ) > 0.9 )
        {
            offsetX = Random.Range( -5.0f, 5.0f );
            offsetZ = Random.Range( -5.0f, 5.0f );
        }

        Vector3 target = _Player.transform.position;
        target.x += offsetX;
        target.z += offsetZ;

        Vector3 motionVector = (Vector3.Lerp( transform.position, target, speedScaler * Time.deltaTime)) - transform.position;
        
        _controller.Move( motionVector );
    }

    private void castShadow( GameObject castingObject, GameObject shadow )
    {
        RaycastHit hit;

        Vector3 castingPosition = castingObject.transform.position;
        Vector3 castingDirection = castingObject.transform.TransformDirection(Vector3.down);

        if (Physics.Raycast( castingPosition, castingDirection, out hit, Mathf.Infinity ))
        {
            Debug.DrawRay( castingPosition, castingDirection * hit.distance, Color.yellow );
            // Debug.Log("Did Hit");

            shadow.transform.position = hit.point;

        }
        else
        {
            Debug.DrawRay( castingPosition, castingDirection * 1000, Color.white);
            // Debug.Log("Did not Hit");
        }
    }

    void followPlayer()
    {

    }

    void AttackLeft()
    {
        // Vector3 attacking = new Vector3( 0, 0, 0);
        // StartCoroutine( AttackLeftCoroutine( pawL, pawLWindingOffset, attacking, attackDelay ) );
        
        isCooledDown = false;
        _animator.SetTrigger( _AttackLeft_trigger );
        StartCoroutine( AttackCooldownCoroutine( cooldownSec ) );
    }

    IEnumerator AttackLeftCoroutine( GameObject attackingObj, Vector3 offsetPos, Vector3 attackingPos, float attackDelay )
    {
        Debug.Log( "start atttack coroutine");
        // winding
        float tick = 0f;
        Vector3 targetOffestPos = calWindingPosition( attackingObj.transform.position , offsetPos );
        while (tick <= attackDelay ) {
            tick += Time.deltaTime * windingSpeed;
            Vector3 newPos = Vector3.Lerp( attackingObj.transform.position, targetOffestPos, tick/attackDelay);
            attackingObj.transform.position = newPos;
            // Debug.Log( "going at " + newPos );
            yield return null;
        }

        // attack
    }

    IEnumerator AttackCooldownCoroutine( float cooldownTime )
    {
        float tick = 0f;
        while( tick < cooldownTime )
        {
            tick += Time.deltaTime;
            yield return null;
        }
        isCooledDown = true;
    }

    void AtackRight()
    {
        isCooledDown = false;
        _animator.SetTrigger( _AttackRight_trigger );
        StartCoroutine( AttackCooldownCoroutine( cooldownSec ) );
    }

    public void ParentTakeDamage( int damage )
    {
        TakeDamage( damage );
    }

    public void TakeDamage( int damage )
    {
        // if(_animator.GetCurrentAnimatorStateInfo(0).IsName("TakingDamage")) return;

        if(currentHp <= 0) 
            return;

        currentHp -= damage;
        currentHp = (int) Mathf.Max((float) currentHp, 0.0f);
        // _animator.SetTrigger( _takeDamage_trigger );
        if(currentHp <= 0) {
            // currentState = StateName.Death;
            _animator.Play("Base Layer.Death", 0, 0 );
            // StartCoroutine( DeathCoroutine( deathDelaySec ) );
        }
        else
        {
            StartCoroutine(ChangeEnemyColour());
        }
    }


    private IEnumerator ChangeEnemyColour() {
        Color32 baseColor = new Color32(255, 255, 255, 255);
        float tick = 0f;
        while (tick <= 2.0f * colorRound) {
            tick += Time.deltaTime * colorFreq;
            Color newColor = Color.Lerp(baseColor, damageColor, Mathf.PingPong(tick, 1));
            foreach ( SpriteRenderer renderer in _renderers )
            {
                renderer.color = newColor;
            } 
            Debug.Log( "color " + newColor );
            Debug.Log(Mathf.PingPong(tick, 1));
            yield return null;
        }
    }

    private Vector3 calWindingPosition( Vector3 currentObjectPos, Vector3 offset )
    {
        return currentObjectPos + offset;
    }

    // protected Vector3 CalAttackPos()
    // {
    //     Vector3 globalAttackPos = transform.position;
    //     globalAttackPos += Vector3.Scale( attackPos_relative,  new Vector3( _facing, 1, 1 ) );

    //     return globalAttackPos;
    // }

    // protected bool Attack( Vector3 attackPos, float attackRadius, int power, string doDamageToTag )
    // {
    //     int maxColider = 10;
    //     Collider[] _hitCollider = new Collider[ maxColider ];
    //     int _numfound = Physics.OverlapSphereNonAlloc( attackPos, attackRadius, _hitCollider );

    //     int hitCounter = 0;
    //     for( int i = 0; i < _numfound ; i++ )
    //     {
    //         if (_hitCollider[i].gameObject.tag == doDamageToTag)
    //         {
    //             hitCounter += 1;
    //             _hitCollider[i].SendMessage( _damage_message, power );
    //         }
            
    //     }

    //     return hitCounter > 0;

    // }

# if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Vector3 WindingPoshead = calWindingPosition( head.transform.position , headWindingOffset );
        Vector3 WindingPospawR = calWindingPosition( pawR.transform.position , pawRWindingOffset );
        Vector3 WindingPospawL = calWindingPosition( pawL.transform.position , pawLWindingOffset );

        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere( WindingPoshead, 2 );
        Gizmos.DrawWireSphere( WindingPospawR, 2 );
        Gizmos.DrawWireSphere( WindingPospawL, 2 );
    }

# endif

}
