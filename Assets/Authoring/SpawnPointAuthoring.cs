using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

public class SpawnPointAuthoring : Authoring<SpawnComponent>
{
    [SerializeField] SpawnType spawnType;

    protected override void Start()
    {
        base.Start();

        entity.Get<SpawnComponent>().spawnType = spawnType;
        entity.Get<SpawnComponent>().pos = new Vector2(transform.position.x, transform.position.z);
        entity.Get<SpawnComponent>().captureValue = 0;
        entity.Get<SpawnComponent>().view = GetComponent<SpawnPoint>();
    }
}
