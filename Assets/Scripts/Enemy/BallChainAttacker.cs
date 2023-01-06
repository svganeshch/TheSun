using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallChainAttacker : EnemyController
{
    public override void InitialiseStart()
    {
        HealthMax = 1000;
        Health = HealthMax;
        Speed = 50;
        DetectionRange = 50;
        DamageAmount = 100;
    }
}
