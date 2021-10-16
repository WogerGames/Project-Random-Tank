using Leopotam.Ecs;
using UnityEngine;

sealed class IncreaseUISystem : IEcsRunSystem
{
    readonly EcsFilter<PlayerComponent, IncreaseComponent> players;
    readonly EcsFilter<PlayerComponent> ppsha;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            players.Get1(p).view.increaseLabel.text = players.Get2(p).ToString();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach (var i in ppsha)
            {
                if (ppsha.Get1(i).view.photonView.IsMine && !ppsha.GetEntity(i).Has<AIControllerComponent>())
                {
                    ppsha.GetEntity(i).Get<HealthPointComponent>().Value += 100;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.T)) 
        {
            foreach (var i in ppsha)
            {
                if (ppsha.Get1(i).teamNum == TeamNum.Two)
                {
                    ppsha.GetEntity(i).Get<HealthPointComponent>().Value += 10;
                }
            }
        }
    }
}
