using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableAttack
{
    void setIsPaused(bool isPaused);
    void RunStart();
    void RunUpdate();
    void TargetStart();
    void TargetUpdate();
    void Attack();
}
