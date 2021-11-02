using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBase
{
    GameObject gameObject { get; }

    void Collision(int Side);

    void HurtPlayer();
    void ResetEnemy();

    void DisableEnemy();
}
