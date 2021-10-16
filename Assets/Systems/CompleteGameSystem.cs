using Leopotam.Ecs;
using UnityEngine;

sealed class CompleteGameSystem : IEcsRunSystem
{
    // auto-injected fields.
    readonly EcsWorld _world = null;
    readonly PlayerManager playerManager;
    readonly EcsFilter<CompleteGameEvent> complete;
    readonly EcsFilter<PlayerComponent>.Exclude<AIControllerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var c in complete)
        {

            if (playerManager.team == complete.Get1(c).winningTeam)
            {
                playerManager.cards += 8;
                Debug.Log("«‡Ë·Ë˛Òˇ");
            }
            else
            {
                playerManager.cards += 3;
                Debug.Log("ÿ“¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿?????!!!!!");
            }

        }
    }
}
