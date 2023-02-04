using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBase: IMoveableAttack
{
    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject enemy;
    [SerializeField] private float speedScaler = 1.0f;
    [SerializeField] private AnimationCurve speedCurve;

    protected Vector3 targetPosition;

    public MovementBase (GameObject c_player, GameObject c_enemy, float c_speedScaler, AnimationCurve c_speedCurve) {
        player = c_player;
        enemy = c_enemy;
        speedScaler = c_speedScaler;
        speedCurve = c_speedCurve;
    }
    public void RunStart()
    {
        // Default value
        targetPosition = enemy.transform.position;
        TargetStart();
    }

    // Update is called once per frame
    public void RunUpdate()
    {
        TargetUpdate(); // Movement target logic
        Attack(); // Attack logic

        // Movement control
        float speed = speedCurve.Evaluate(Vector3.Distance(enemy.transform.position, targetPosition));
        enemy.transform.position = Vector3.Lerp(enemy.transform.position, targetPosition, speedScaler * speed * Time.deltaTime);
    }

    public abstract void TargetStart();

    // TODO: Implement target position update
    public abstract void TargetUpdate();

    // TODO Implement attack state
    public abstract void Attack();
}
