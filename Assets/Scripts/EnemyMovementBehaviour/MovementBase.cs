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
    protected bool isPaused = false;

    public MovementBase (GameObject c_player, GameObject c_enemy, float c_speedScaler, AnimationCurve c_speedCurve) {
        player = c_player;
        enemy = c_enemy;
        speedScaler = c_speedScaler;
        speedCurve = c_speedCurve;
    }

    public void setIsPaused(bool isPaused) {
        this.isPaused = isPaused;
    }

    protected Vector3 validTarget(Vector3 position) {
        return new Vector3(position.x, enemy.transform.position.y, position.z);
    }

    public void RunStart()
    {
        targetPosition = validTarget(enemy.transform.position); // Default value
        TargetStart();
        targetPosition = validTarget(targetPosition);
    }

    // Update is called once per frame
    public void RunUpdate()
    {
        if(isPaused) {
            targetPosition = validTarget(enemy.transform.position); // Reset target transform
        } else {
            TargetUpdate(); // Movement target logic
            targetPosition = validTarget(targetPosition);

            Attack(); // Attack logic
        }
        
        // Movement control
        float speed = speedCurve.Evaluate(Vector3.Distance(enemy.transform.position, targetPosition));

        CharacterController enemy_controller = enemy.GetComponent("CharacterController") as CharacterController;
        enemy_controller.Move(
            Vector3.Lerp(enemy.transform.position, targetPosition, speedScaler * speed * Time.deltaTime)
            - enemy.transform.position
        );
    }

    public abstract void TargetStart();

    // TODO: Implement target position update
    public abstract void TargetUpdate();

    // TODO Implement attack state
    public abstract void Attack();
}
