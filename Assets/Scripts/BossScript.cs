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

    [SerializeField] private Vector3 pawLTarget;
    [SerializeField] private Vector3 pawRTarget;
    [SerializeField] private float attackRadius;

    public float attackDelayL = 0.2f;
    public float attackDelayR = 0.2f;
    public int attackPower = 2;

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
    private string _damage_message = "TakeDamage";

    private GameObject _Player;

    private float offsetX = 0f;
    private float offsetZ = 0f;

    private bool playerInBoundsL = false;
    private bool playerInBoundsR = false;

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
                    AttackRight();
            }
        }

        if ( _Player == null )
            _Player = GameManager.Instance.getPlayer();


    }

    private void FollowPlayer()
    {
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

    void AttackLeft()
    {
        // Vector3 attacking = new Vector3( 0, 0, 0);
        // StartCoroutine( AttackLeftCoroutine( pawL, pawLWindingOffset, attacking, attackDelay ) );
        
        isCooledDown = false;
        _animator.SetTrigger( _AttackLeft_trigger );
        StartCoroutine( AttackCooldownCoroutine( cooldownSec ) );

        Vector3 pawLTargetPos = CalAttackPos( pawL, pawLTarget );
        StartCoroutine( AttackDelayCoroutine( pawLTargetPos, attackPower, attackDelayL, "L" ) );
    }

    void AttackRight()
    {
        isCooledDown = false;
        _animator.SetTrigger( _AttackRight_trigger );
        StartCoroutine( AttackCooldownCoroutine( cooldownSec ) );

        Vector3 pawRTargetPos = CalAttackPos( pawR, pawRTarget );
        StartCoroutine( AttackDelayCoroutine( pawRTargetPos, attackPower, attackDelayR, "R" ) );
    }

    IEnumerator AttackLeftCoroutine( GameObject attackingObj, Vector3 offsetPos, Vector3 attackingPos, float attackDelay )
    {
        Debug.Log( "start atttack coroutine");
        // winding
        float tick = 0f;
        Vector3 targetOffestPos = CalAttackPos( attackingObj, offsetPos );
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

    IEnumerator AttackDelayCoroutine( Vector3 targetPos, int pow , float attackDelay, string side )
    {
        float tick = 0f;
        while( tick < attackDelay )
        {
            tick += Time.deltaTime;
            yield return null;
        }

        // Attack( targetPos, pow, "Player" );
        if ( side == "L" && playerInBoundsL )
            _Player.SendMessage( _damage_message, pow );
        
        if ( side == "R" && playerInBoundsR )
            _Player.SendMessage( _damage_message, pow );
    }

    // protected void Attack( int power, string doDamageToTag )
    // {

    //     if ( playerInBounds )
    //         _Player.SendMessage( _damage_message, power );

    // }

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

    private Vector3 CalAttackPos( GameObject attackingObject, Vector3 offset )
    {
        return attackingObject.transform.position + offset;
    }

    public void PlayerInBoundL()
    {
        playerInBoundsL = true;
    }

    public void PlayerInBoundR()
    {
        playerInBoundsR = true;
    }

    public void PlayerExitBound()
    {
        playerInBoundsR = false;
        playerInBoundsL = false;
    }

# if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 pawLTargetPos = CalAttackPos( pawL, pawLTarget );
        Vector3 pawRTargetPos = CalAttackPos( pawR, pawRTarget );

        Gizmos.DrawWireSphere( pawLTargetPos, attackRadius );
        Gizmos.DrawWireSphere( pawRTargetPos, attackRadius );
    }

# endif

    // void OnTriggerStay(Collider Other){
    //     if( Other.gameObject.tag == "Player"){
    //         playerInBounds = true;
    //         Debug.Log( "Player Enter");
    //     }
    // }
    
    // void OnTriggerExit(Collider Other){
    //     if( Other.gameObject.tag == "Player"){
    //         playerInBounds = false;
    //     }
    // }

}
