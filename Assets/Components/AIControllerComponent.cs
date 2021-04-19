using UnityEngine;
using Leopotam.Ecs;

public struct AIControllerComponent
{
    public float timerToCheckState;
    public Vector2 moveTarget;
    public Transform enemyTarget;
}
