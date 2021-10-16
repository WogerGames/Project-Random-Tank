using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.AI;

sealed class AIMoveDirectionSystem : IEcsRunSystem
{
    //readonly EcsFilter<PlayerComponent, MoveComponent, AIControllerComponent> players;
    readonly EcsFilter<PlayerComponent, AIControllerComponent> players;

    public void Run()
    {
        foreach (var p in players)
        {
            if (!MultiplayerManager.IsMaster)
                break;

            ref var ai = ref players.Get2(p);

            var agent = players.Get1(p).view.GetComponent<NavMeshAgent>();

            if (ai.moveTarget == Vector2.zero)
            {
                if(agent.isOnNavMesh)
                    agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;

                Vector3 desteny = new Vector3(ai.moveTarget.x, 0, ai.moveTarget.y);

                agent.SetDestination(desteny);
            }
            
        }
    }



    //void IEcsRunSystem.Run()
    //{
    //    foreach (var p in players)
    //    {
    //        ref var ai = ref players.Get3(p);

    //        if (ai.moveTarget == Vector2.zero)
    //            continue;

    //        var t = players.Get1(p).view.transform;
    //        Vector2 dir = ai.moveTarget - new Vector2(t.position.x, t.position.z);

    //        dir.Normalize();

    //        ref var move = ref players.Get2(p);

    //        move.Value = dir * Time.deltaTime * move.Speed;
    //    }
    //}
}
