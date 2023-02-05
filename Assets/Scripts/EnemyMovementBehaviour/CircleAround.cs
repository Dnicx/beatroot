using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAround : MovementBase
{
    public int maxEnemies = 5;

    // Circle Settings
    private float minRadius = 6.0f;
    private float maxRadius = 12.0f;
    private float angleScaler = 1.0f;
    private float decayRate = 0.7f;

    // Internal states
    private float circleRadius = 5.0f;
    private float angle = 0.0f;

    private float attckChance = 0.5f;
    private float attackRadiusRatio = 1.0f;
    private float attackInterval = 1.0f;
    private float attackNotROffset = 0.8f;
    private float tick = 0.0f;
    private bool justGotAttacked = false;
    private int direction = 1;


    public CircleAround (
        GameObject c_player, GameObject c_enemy,
        float c_speedScaler, AnimationCurve c_speedCurve,

        int c_maxEnemies = 5,
        float c_minRadius = 6.0f, float c_maxRadius = 12.0f,
        float c_angleScaler = 1.0f, float c_decayRate = 0.7f,

        float c_attckChance = 0.5f, float c_attackRadiusRatio = 1.0f,
        float c_attackInterval = 1.0f, float c_attackNotROffset = 0.8f
    ) : base(c_player, c_enemy, c_speedScaler, c_speedCurve) {
        maxEnemies = c_maxEnemies;
        minRadius = c_minRadius;
        maxRadius = c_maxRadius;
        angleScaler = c_angleScaler;
        decayRate = c_decayRate;

        attckChance = c_attckChance;
        attackRadiusRatio = c_attackRadiusRatio;
        attackInterval = c_attackInterval;
        attackNotROffset = c_attackNotROffset;

        direction = Random.Range(0.0f, 1.0f) < 0.5f ? -1 : 1;
    }

    public override void TargetStart() {
        circleRadius = maxRadius;
    }
    
    public override void TargetUpdate() {
        Vector3 circleCenter = player.transform.position;
        angle = (angle + Time.deltaTime) % (2 * Mathf.PI);
        float angleDeg = angleScaler * angle * 180.0f / Mathf.PI;

        bool isBackOff = IsBackOff();

        // Distance based condition
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        if(distance > maxRadius) {
            targetPosition = circleCenter;
        } else {
            targetPosition = circleCenter + Quaternion.Euler(0, angleScaler * angleDeg, 0) * new Vector3(0, 0, direction * -circleRadius);
            
            if(isBackOff) {
                Debug.Log(isBackOff);
                circleRadius += 2 * decayRate * Time.deltaTime;
            } else {
                if (distance < maxRadius) {
                    circleRadius -= decayRate * Time.deltaTime;
                }
            }
            
            circleRadius = Mathf.Max(Mathf.Min(maxRadius, circleRadius), minRadius);
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
        // TODO Attack if in range

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

        /*
        bool isAttack = Random.Range(0.0f, 1.0f) < attckChance;
        if(isAttack && Vector3.Distance(enemy.transform.position, player.transform.position) <= attackRadiusRatio * minRadius) {
            enemy.GetComponent<Enemy>().Attack();
        } */

        /*
        if(enemy.GetComponent<Enemy>().currentState == Enemy.StateName.TakeDamage) {
            justGotAttacked = true;
        }
        tick += Time.deltaTime;
        if(tick > attackInterval || (
            justGotAttacked && enemy.GetComponent<Enemy>().currentState != Enemy.StateName.TakeDamage
        )) {
            bool isAttack = Random.Range(0.0f, 1.0f) < attckChance;
            if(isAttack) {
                tick = 0.0f;
                if(Vector3.Distance(enemy.transform.position, player.transform.position) <= attackRadiusRatio * minRadius) {
                    enemy.GetComponent<Enemy>().Attack();
                }
                justGotAttacked = false;
            } else {
                tick = attackNotROffset * attackInterval;
            }
        }
        */
    }
}
