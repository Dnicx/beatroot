using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlTarget : MovementBase
{   
    // always slowly walking toward player no matter where the player is
    // But stop at minRadius
    
    // Settings
    private float minRadius = 1.0f;
    
    // Internal state
    private bool isReached = false;

    public CrawlTarget (
        GameObject c_player, GameObject c_enemy,
        float c_speedScaler, AnimationCurve c_speedCurve,
        float c_minRadius = 1.0f
    ) : base(c_player, c_enemy, c_speedScaler, c_speedCurve) {
        minRadius = c_minRadius;
    }

    public override void TargetStart() {
        targetPosition = player.transform.position;
        isReached = false;
    }
    
    public override void TargetUpdate() {
        if (Vector3.Distance(enemy.transform.position, player.transform.position) <= minRadius) {
            isReached = true;
            targetPosition = enemy.transform.position;
        } else {
            targetPosition = player.transform.position;
        }
    }

    public override void Attack() {

    }
}
