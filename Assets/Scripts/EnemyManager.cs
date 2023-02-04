using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager 
{   
    // TODO
    // [ ] Keep track of enemy spawning (not too much)
    //      Soft-core difficulty level (not really)
    // [ ] Add signal (public variable) for GameManager s.t. it can know
    //      When the enemy scene had complete (signalling change to next scene)
    // [ ] Add instance pool (reuse)

    [SerializeField] public Enemy[] allEnemies = new Enemy[] {};
    
}
