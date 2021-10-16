using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;


public class HealthPointAuthoring : Authoring<HealthPointComponent>
{
    [SerializeField] public int MaxValue = 30;

    protected override void Start()
    {
        base.Start();

        entity.Get<HealthPointComponent>().MaxValue = MaxValue;
    }
}
