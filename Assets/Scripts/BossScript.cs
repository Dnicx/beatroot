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

    [SerializeField] private float deathDelaySec = 1.0f;
    [SerializeField] private Color32 damageColor;
    [SerializeField] private float colorFreq = 5.0f;
    [SerializeField] private int colorRound = 3;
    private Color32 startColor;
    private Color32 endColor;

    private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
    private Animator _animator;

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
    }

    // Update is called once per frame
    void Update()
    {
        castShadow( head, headShadow );
        castShadow( pawR, pawRShadow );
        castShadow( pawL, pawLShadow );
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
        Vector3 attacking = new Vector3( 0, 0, 0);
        _animator.SetTrigger( "Attacking" );
        StartCoroutine( AttackLeftCoroutine( pawL, pawLWindingOffset, attacking, attackDelay ) );
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

    void AtackRight()
    {

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
