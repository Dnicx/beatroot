using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlTarget : MovementBase
{   
    // always slowly walking toward player no matter where the player is
    // But stop at minRadius
    
    // Settings
    private int maxEnemies = 5;
    private float minRadius = 1.0f;
    private float attckChance = 0.5f;
    private float attackRadiusRatio = 1.0f;
    private float attackInterval = 1.0f;
    private float attackNotROffset = 0.8f;
    private float tick = 0.0f;
    
    // Internal state
    private bool isReached = false;
    private Vector3 originalPos;

    public CrawlTarget (
        GameObject c_player, GameObject c_enemy,
        float c_speedScaler, AnimationCurve c_speedCurve,

        int c_maxEnemies = 5, float c_minRadius = 1.0f,

        float c_attckChance = 0.5f, float c_attackRadiusRatio = 1.0f,
        float c_attackInterval = 1.0f, float c_attackNotROffset = 0.8f
    ) : base(c_player, c_enemy, c_speedScaler, c_speedCurve) {
        maxEnemies = c_maxEnemies;
        minRadius = c_minRadius;

        attckChance = c_attckChance;
        attackRadiusRatio = c_attackRadiusRatio;
        attackInterval = c_attackInterval;
        attackNotROffset = c_attackNotROffset;
    }

    public override void TargetStart() {
        targetPosition = player.transform.position;
        isReached = false;
        originalPos = enemy.transform.position;
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
    
    public override void TargetUpdate() {
        bool isBackOff = IsBackOff();

        if(isBackOff) {
            isReached = false;
            float normDist = Mathf.Max(Vector3.Distance(originalPos, enemy.transform.position), 1.0f);
            targetPosition = (originalPos - enemy.transform.position) / normDist + enemy.transform.position;
        } else if (Vector3.Distance(enemy.transform.position, player.transform.position) <= minRadius) {
            isReached = true;
            targetPosition = enemy.transform.position;
        } else {
            targetPosition = player.transform.position;
        }
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
