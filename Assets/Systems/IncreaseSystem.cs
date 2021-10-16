using Leopotam.Ecs;
using UnityEngine;

sealed class IncreaseSystem : IEcsRunSystem
{
    readonly EcsFilter<ProjectileCollisionEvent> colEvents;
    readonly EcsFilter<PlayerComponent, IncreaseComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var c in colEvents)
        {
            foreach (var p in players)
            {
                if(colEvents.Get1(c).damageData.OwnerId == players.Get1(p).view.photonView.ViewID)
                {
                    //Debug.Log("НАшли плауера, ёёба");
                    var increase = players.Get2(p).Value * 3;
                    colEvents.Get1(c).damageData.Damage += increase;
                }
            }
        }
    }
}
