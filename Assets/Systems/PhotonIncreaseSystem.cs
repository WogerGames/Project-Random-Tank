using Leopotam.Ecs;


sealed class PhotonIncreaseSystem : IEcsRunSystem
{
    readonly EcsFilter<PlayerComponent, IncreaseComponent, IncreasedEvent> players;

    void IEcsRunSystem.Run()
    {
        // Отправка другим игрокам инфы об усилении
        foreach (var p in players)
        {
            if (players.Get1(p).view.photonView.IsMine)
            {
                var value = players.Get2(p).Value;
                players.Get1(p).view.IncreaseChoosed(value);
            }
        }
    }
}
