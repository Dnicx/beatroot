using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableAttack
{
    bool getIsPaused();
    void setIsPaused(bool isPaused);
    void RunStart();
    Vector3 RunUpdate();
    void TargetStart();
    void TargetUpdate();
    void Attack();
}
