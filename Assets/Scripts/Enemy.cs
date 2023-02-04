using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{

    // TODO add enemy state e.g. walk, run, ...
    private Animator _animator;
    private SpriteRenderer _renderer;
    private CharacterController _controller;

    private string _takeDamage_trigger = "takeDamage";
    private string _punch_trigger = "punch01";
    private string _animatorBool_walk = "walk";

    private Vector3 lastPos;

    // Start is called before the first frame update
    [SerializeField] public GameObject player;
    [SerializeField] private float deathDelaySec = 1.0f;
    [SerializeField] private float speedScaler = 1.0f;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float knockBackScaler = 0.0f;

    // Movement bahaviour specific settings
    [SerializeField] public int maxEnemies = 5;
    [SerializeField] private float bufferTime = 3.0f;
    [SerializeField] private float minRadius = 6.0f;
    [SerializeField] private float maxRadius = 12.0f;
    [SerializeField] private float angleScaler = 1.0f;
    [SerializeField] private float decayRate = 0.7f;
    [SerializeField] private MovementType behaviourType = MovementType.CircleAround;
    [SerializeField] private Color32 damageColor;
    [SerializeField] private float colorFreq = 5.0f;
    [SerializeField] private int colorRound = 3;
    private Color32 startColor;
    private Color32 endColor;
    

    enum MovementType {
        CircleAround,
        CrawlTarget,
        TargetSpotUpdate
    };

    [SerializeField] private double distanceEps = 0.005;
    public enum StateName {
        Idle, Walk, TakeDamage, Attack, Death
    }
    public StateName currentState;
    public StateName lastState;
    
    private bool lockState = false;

    private IMoveableAttack m_currentBehaviour;
    private string[] attackStateNames = new string[] {
        "Punch01"
    };

    public IMoveableAttack currentBehaviour {
        get => m_currentBehaviour;
        set => m_currentBehaviour = value;
    }

    void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
        _controller = GetComponent<CharacterController>();
        endColor = new Color32(255, 255, 255, 255);
        currentHp = maxHp;
        currentState = StateName.Idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        lastPos = transform.position;

        // GameObject - enemy movement + attack pattern
        switch(behaviourType) {
            case MovementType.CircleAround:
                m_currentBehaviour = new CircleAround(player, gameObject, speedScaler, speedCurve, minRadius, maxRadius, angleScaler, decayRate);
                break;
            case MovementType.CrawlTarget:
                m_currentBehaviour = new CrawlTarget(player, gameObject, speedScaler, speedCurve, minRadius);
                break;
            case MovementType.TargetSpotUpdate:
                m_currentBehaviour = new TargetSpotUpdate(player, gameObject, speedScaler, speedCurve, maxEnemies, minRadius, maxRadius, bufferTime);
                break;
        }
        m_currentBehaviour.RunStart();   
    }

    public void StartWalk() {
        _animator.SetBool( _animatorBool_walk, true );
    }

    public void StopWalk() {
        _animator.SetBool( _animatorBool_walk, false );
    }

    public void TakeDamage( int damage )
    {
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("TakingDamage")) return;

        currentHp -= damage;
        currentHp = (int) Mathf.Max((float) currentHp, 0.0f);
        _animator.SetTrigger( _takeDamage_trigger );
        if(currentHp <= 0) {
            currentState = StateName.Death;
            _animator.Play("Base Layer.Death", 0, 0 );
            StartCoroutine( DeathCoroutine( deathDelaySec ) );
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
            _renderer.color = Color.Lerp(baseColor, damageColor, Mathf.PingPong(tick, 1));
            Debug.Log(Mathf.PingPong(tick, 1));
            yield return null;
        }
    }

    public void Attack()
    {
        // TODO: check player hitbox
        
        _animator.SetTrigger( _punch_trigger );
        m_currentBehaviour.setIsPaused(true);
        currentState = StateName.Attack;
        lockState = true;
    }

    void Update() {
        // State Check

        bool isMatched = false;
        foreach(string animationStateName in attackStateNames) {
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName)) {
                isMatched = true;
                break;
            }
        }

        if(isMatched) {
            // Attack
            m_currentBehaviour.setIsPaused(true);
            currentState = StateName.Attack;
            lockState = true;
        } else if(_animator.GetCurrentAnimatorStateInfo(0).IsName("TakingDamage")) {
            // TakeDamage
            m_currentBehaviour.setIsPaused(true);
            currentState = StateName.TakeDamage;
            lockState = true;
            _controller.Move(
                (player.transform.position.x - transform.position.x <= 0 ? 1 : -1) * new Vector3(knockBackScaler * 1.0f, 0.0f, 0.0f) * Time.deltaTime
            );
        } else {
            // Idle, Walk, Death
            m_currentBehaviour.setIsPaused(false);
            lockState = false;

            Vector3 motionVector = m_currentBehaviour.RunUpdate();
            _controller.Move(motionVector);

            if(
                Vector3.Distance(motionVector, new Vector3(0, 0, 0)) > distanceEps &&
                Vector3.Distance(transform.position, lastPos) > distanceEps
            ) { // Walk
                currentState = StateName.Walk;
                StartWalk();
            } else { // Idle
                currentState = StateName.Idle;
            }
        }

        if(lastState == StateName.Walk && currentState != StateName.Walk) {
            StopWalk();
        }

        _renderer.flipX = player.transform.position.x - transform.position.x > 0;
        lastPos = transform.position;
        lastState = currentState;
    }

    IEnumerator DeathCoroutine( float delaySec )
    {
        float tick = 0f;
        while (tick <= delaySec) {
            tick += Time.deltaTime;
            Debug.Log(Mathf.PingPong(tick, 1));
            yield return null;
        }

        Destroy( gameObject );
    }
}
