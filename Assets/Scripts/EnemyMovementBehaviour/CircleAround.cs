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

    public CircleAround (
        GameObject c_player, GameObject c_enemy,
        float c_speedScaler, AnimationCurve c_speedCurve,

        float c_minRadius = 6.0f, float c_maxRadius = 12.0f,
        float c_angleScaler = 1.0f, float c_decayRate = 0.7f
    ) : base(c_player, c_enemy, c_speedScaler, c_speedCurve) {
        minRadius = c_minRadius;
        maxRadius = c_maxRadius;
        angleScaler = c_angleScaler;
        decayRate = c_decayRate;
    }

    public override void TargetStart() {
        circleRadius = maxRadius;
    }
    
    public override void TargetUpdate() {
        Vector3 circleCenter = player.transform.position;
        angle = (angle + Time.deltaTime) % (2 * Mathf.PI);
        float angleDeg = angleScaler * angle * 180.0f / Mathf.PI;

        // Distance based condition
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        if(distance > maxRadius) {
            targetPosition = circleCenter;
        } else {
            targetPosition = circleCenter + Quaternion.Euler(0, angleScaler * angleDeg, 0) * new Vector3(0, 0, -circleRadius);
            if (distance < maxRadius) {
                circleRadius -= decayRate * Time.deltaTime;
            }
            circleRadius = Mathf.Max(Mathf.Min(maxRadius, circleRadius), minRadius);
        }
    }

    public override void Attack() {
        // TODO Attack if in range
        // If attack - stop target position
    }
}
