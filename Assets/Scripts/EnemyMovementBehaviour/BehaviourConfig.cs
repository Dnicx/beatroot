using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourConfigTypes {
    public static string circleAround_fast = "CircleAround_fast";
    public static string circleAround_normal = "CircleAround_normal";
    public static string circleAround_slow = "CircleAround_normal";
    public static string crawlTarget_fast = "CrawlTarget_fast";
    public static string crawlTarget_normal = "CrawlTarget_normal";
    public static string crawlTarget_normal_close = "CrawlTarget_normal_close";
    public static string targetSpot_fast = "TargetSpot_fast";
    public static string targetSpot_normal = "TargetSpot_normal";
    public static string targetSpot_slow = "TargetSpot_slow";
}

[CreateAssetMenu(fileName = "BehaviourConfig", menuName = "BehaviourConfig", order = 0)]
public class BehaviourConfig : ScriptableObject {
    public string mode;
    public int maxEnemies;
    public float bufferTime;
    public float speedScaler;
    public float minRadius;
    public float maxRadius;
    public float angleScaler;
    public float decayRate;
    public MovementType behaviourType;
    public AnimationCurve speedCurve;
    public float knockBackScaler;
    public Color32 damageColor;

    public IMoveableAttack getMovementBehaviour(GameObject player, GameObject enemy) {
        switch(behaviourType) {
            case MovementType.CircleAround:
                return new CircleAround(player, enemy, speedScaler, speedCurve, maxEnemies, minRadius, maxRadius, angleScaler, decayRate);
            case MovementType.CrawlTarget:
                return new CrawlTarget(player, enemy, speedScaler, speedCurve, maxEnemies, minRadius);
            case MovementType.TargetSpotUpdate:
                return new TargetSpotUpdate(player, enemy, speedScaler, speedCurve, maxEnemies, minRadius, maxRadius, bufferTime);
        }
        return null;
    }

    public string getCode() {
        string prefix = "";
        switch(behaviourType) {
            case MovementType.CircleAround:
                prefix = "CircleAround";
                break;
            case MovementType.CrawlTarget:
                prefix = "CrawlTarget";
                break;
            case MovementType.TargetSpotUpdate:
                prefix = "TargetSpot";
                break;
        }
        return prefix + "_" + mode;
    }
}
