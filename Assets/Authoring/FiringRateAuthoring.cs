using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

public class FiringRateAuthoring : Authoring<FiringRateComponent>
{
    [SerializeField] public float Vaule;

    protected override void Start()
    {
        base.Start();

        entity.Get<FiringRateComponent>().Value = Vaule;
    }
}
