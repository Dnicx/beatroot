using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableAttack
{
    void RunStart();
    void RunUpdate();
    void TargetStart();
    void TargetUpdate();
    void Attack();
}
