using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpotUpdate : MovementBase
{
    // Settings
    public int maxEnemies = 5;
    private float minRadius = 1.0f;
    private float maxRadius = 12.0f;
    private float bufferTime = 3.0f;
    
    // Internal States
    private bool isReached = false;
    private float bufferCounter = 0.0f;

    // Attack thing
    private float attckChance = 0.5f;
    private float attackRadiusRatio = 1.0f;
    private float attackInterval = 1.0f;
    private float attackNotROffset = 0.8f;
    private float tick = 0.0f;

    public TargetSpotUpdate (
        GameObject c_player, GameObject c_enemy,
        float c_speedScaler, AnimationCurve c_speedCurve,

        int c_maxEnemies = 5, float c_minRadius = 1.0f,
        float c_maxRadius = 12.0f, float c_bufferTime = 3.0f,

        float c_attckChance = 0.5f, float c_attackRadiusRatio = 1.0f,
        float c_attackInterval = 1.0f, float c_attackNotROffset = 0.8f
    ) : base(c_player, c_enemy, c_speedScaler, c_speedCurve) {
        maxEnemies = c_maxEnemies;
        minRadius = c_minRadius;
        maxRadius = c_maxRadius;
        bufferTime = c_bufferTime;

        attckChance = c_attckChance;
        attackRadiusRatio = c_attackRadiusRatio;
        attackInterval = c_attackInterval;
        attackNotROffset = c_attackNotROffset;
    }

    public override void TargetStart() {
        targetPosition = player.transform.position;
        isReached = false;
    }

    public override void TargetUpdate() {
        Vector3 playerCenter = player.transform.position;

        float playerDistance = Vector3.Distance(enemy.transform.position, player.transform.position);
        float targetDistance = Vector3.Distance(enemy.transform.position, targetPosition);

        bool isBackOff = IsBackOff();

        if(targetDistance > maxRadius) {
            // Move toward player
            isReached = false;
        } else if (targetDistance <= minRadius || isBackOff) {
            isReached = true;
        }

        if(isReached) {
            if(bufferCounter > bufferTime) {
                // Retarget new random position
                if(playerDistance > maxRadius) {
                    float randAngle = Random.Range(0, 2*Mathf.PI);
                    targetPosition = playerCenter + minRadius * new Vector3(
                        Mathf.Sin(randAngle), 0, Mathf.Cos(randAngle)
                    );
                } else {
                    if(isBackOff) {
                        targetPosition = playerCenter + new Vector3(
                            (Random.value < 0.5 ? 1 : -1) * Random.Range(maxRadius, 2*maxRadius),
                            0,
                            (Random.value < 0.5 ? 1 : -1) * Random.Range(maxRadius, 2*maxRadius)
                        );
                    } else {
                        targetPosition = playerCenter + new Vector3(
                            (Random.value < 0.5 ? 1 : -1) * Random.Range(minRadius, maxRadius),
                            0,
                            (Random.value < 0.5 ? 1 : -1) * Random.Range(minRadius, maxRadius)
                        );
                    }
                }
                isReached = false;
                bufferCounter = 0.0f;
            } else {
                bufferCounter += Time.deltaTime;
            }
        }
    }
    private bool IsBackOff() {
        // Check if back-off
        GameObject[] results = GameObject.FindGameObjectsWithTag(EntityTagNames.enemyManagerTag);
        List<float> allDistance = null;
        allDistance = EnemyManager.enemiesDistance;

        if(allDistance == null) {
            allDistance =  GameObject.FindGameObjectsWithTag(EntityTagNames.playerTag)[0].GetComponent<PlayerScript>().getEnemiesDistance();   
        }

        int withinRadius = 0;
        foreach(float it in allDistance) {
            if(it <= attackRadiusRatio * minRadius) withinRadius++;
        }
        return withinRadius >= maxEnemies;
    }


    public override void Attack() {
        tick += Time.deltaTime;
        if(tick > attackInterval) {
            bool isAttack = Random.Range(0.0f, 1.0f) < attckChance;
            if(isAttack && (
                Vector3.Distance(enemy.transform.position, player.transform.position) <= attackRadiusRatio * minRadius
            )) {
                enemy.GetComponent<Enemy>().Attack();
                tick = 0.0f;
            } else {
                tick = attackNotROffset * attackInterval;
            }
        }
    }
}
