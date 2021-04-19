using Leopotam.Ecs;


sealed class Perk3System : IEcsRunSystem
{
    readonly EcsFilter<PlayerComponent, Perk3> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            players.Get1(p).damage++;
            players.Get1(p).damage++;
            players.Get1(p).damage++;
            UnityEngine.Debug.Log("Œ√ŒÕ‹ " + players.Get1(p).damage);
        }
    }
}
