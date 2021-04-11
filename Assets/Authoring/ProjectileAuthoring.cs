using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

public class ProjectileAuthoring : Authoring<ProjectileComponent>
{
    protected override void Start()
    {
        base.Start();

        entity.Get<ProjectileComponent>().view = GetComponent<Projectile>();
    }
}
