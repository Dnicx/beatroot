using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implement minion AI
// - Require movement behaviour
public class EnemyMinion : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject enemy;
    [SerializeField] private float speedScaler = 1.0f;
    [SerializeField] private AnimationCurve speedCurve;

    // Movement bahaviour specific settings
    [SerializeField] public int maxEnemies = 5;
    [SerializeField] private float bufferTime = 3.0f;
    [SerializeField] private float minRadius = 6.0f;
    [SerializeField] private float maxRadius = 12.0f;
    [SerializeField] private float angleScaler = 1.0f;
    [SerializeField] private float decayRate = 0.7f;

    private IMoveableAttack m_currentBehaviour;

    public IMoveableAttack currentBehaviour {
        get => m_currentBehaviour;
        set => m_currentBehaviour = value;
    }

    void Start()
    {
        // GameObject - enemy movement + attack pattern
        // m_currentBehaviour = new CrawlTarget(player, enemy, speedScaler, speedCurve, minRadius);
        m_currentBehaviour = new CircleAround(player, enemy, speedScaler, speedCurve, minRadius, maxRadius, angleScaler, decayRate);
        // m_currentBehaviour = new TargetSpotUpdate(player, enemy, speedScaler, speedCurve, maxEnemies, minRadius, maxRadius, bufferTime);
        m_currentBehaviour.RunStart();   
    }

    // Update is called once per frame
    void Update()
    {
        m_currentBehaviour.RunUpdate();
    }
}
