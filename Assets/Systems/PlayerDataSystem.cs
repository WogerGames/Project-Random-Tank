using Leopotam.Ecs;


sealed class PlayerDataSystem : IEcsRunSystem
{
    readonly PlayerManager playerManager;
    readonly EcsFilter<PlayerComponent>.Exclude<AIControllerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            ref var player = ref players.Get1(p);

            if (player.view.photonView.IsMine)
            {
                playerManager.team = player.teamNum;
            }
        }
    }
}
