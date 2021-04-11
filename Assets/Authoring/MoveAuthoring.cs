using Leopotam.Ecs;
using UnityEngine;

public class MoveAuthoring : Authoring<MoveComponent>
{
    [SerializeField] int speed;

    protected override void Start()
    {
        base.Start();

        entity.Get<MoveComponent>().transform = transform;
        entity.Get<MoveComponent>().Speed = speed;
    }
} 
