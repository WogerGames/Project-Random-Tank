using Leopotam.Ecs;


sealed class DestroyedCounter : IEcsRunSystem
{
    readonly EcsFilter<DestroyEvent> events;
    readonly EcsFilter<PlayerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var e in events)
        {
            foreach (var p in players)
            {
                ref var player = ref players.Get1(p);

                if(player.view.photonView.ViewID == events.Get1(e).DestroyerID)
                {
                    player.countDestroyed++;
                    //UnityEngine.Debug.Log($"{player.view.photonView.ViewID} - {player.countDestroyed}");
                }
            }
        }
    }
}
