using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

[DefaultExecutionOrder(300)]
public class Authoring<T> : MonoBehaviour where T : struct
{
    protected EcsWorld World => GameManager.Instance.EcsWorld;
    protected EcsEntity entity = EcsEntity.Null;

    protected virtual void Start()
    {
        var filter = World.GetFilter(typeof(EcsFilter<ObjectComponent>));

        foreach (var i in filter)
        {
            if(filter.GetEntity(i).Get<ObjectComponent>().go == gameObject)
            {
                entity = filter.GetEntity(i);
            }
        }

        if (entity.IsNull())
        {
            entity = World.NewEntity();
            entity.Get<ObjectComponent>().go = gameObject;
            entity.Get<T>();
        }
        else
        {
            entity.Get<T>();
        }
    }
}
