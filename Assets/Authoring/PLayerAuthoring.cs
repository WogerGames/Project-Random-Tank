using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

public class PLayerAuthoring : Authoring<PLayerComponent>
{
    protected override void Start()
    {
        base.Start();

        entity.Get<PLayerComponent>().view = GetComponent<Player>();
    }
}
