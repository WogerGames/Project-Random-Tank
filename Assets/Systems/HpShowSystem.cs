using Leopotam.Ecs;


sealed class HpShowSystem : IEcsRunSystem
{
    EcsFilter<PlayerComponent, HealthPointComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            players.Get1(p).view.hpText.text = players.Get2(p).Value.ToString();
        }
    }
}
