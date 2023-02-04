using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{

    private Animator _animator;

    private string _takeDamage_trigger = "takeDamage";
    private string _punch_trigger = "punch01";

    // Start is called before the first frame update
    [SerializeField] public GameObject player;
    [SerializeField] private float speedScaler = 1.0f;
    [SerializeField] private AnimationCurve speedCurve;

    // Movement bahaviour specific settings
    [SerializeField] public int maxEnemies = 5;
    [SerializeField] private float bufferTime = 3.0f;
    [SerializeField] private float minRadius = 6.0f;
    [SerializeField] private float maxRadius = 12.0f;
    [SerializeField] private float angleScaler = 1.0f;
    [SerializeField] private float decayRate = 0.7f;
    [SerializeField] private MovementType behaviourType = MovementType.CircleAround;

    enum MovementType {
        CircleAround,
        CrawlTarget,
        TargetSpotUpdate
    };

    private IMoveableAttack m_currentBehaviour;

    public IMoveableAttack currentBehaviour {
        get => m_currentBehaviour;
        set => m_currentBehaviour = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

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

    public void TakeDamage( int damage )
    {
        currentHp -= damage;
        _animator.SetTrigger( _takeDamage_trigger );
    }

    public void Attack()
    {
        // TODO: check player hitbox
        
        _animator.SetTrigger( _punch_trigger );

    }

    void Update() {
        m_currentBehaviour.RunUpdate();
    }
}
