using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BehaviourConfig", menuName = "BehaviourConfig", order = 0)]
public class BehaviourConfig : ScriptableObject {
    public int maxEnemies;
    public float bufferTime;
    public float minRadius;
    public float maxRadius;
    public float angleScaler;
    public float decayRate;
    public MovementType behaviourType;
    
}
