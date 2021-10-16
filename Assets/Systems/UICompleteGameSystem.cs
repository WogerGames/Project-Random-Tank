using Leopotam.Ecs;
using Photon.Pun;

sealed class UICompleteGameSystem : IEcsRunSystem
{
    readonly GameManager gameManager;
    readonly EcsFilter<CompleteGameEvent> complete;

    void IEcsRunSystem.Run()
    {
        foreach (var c in complete)
        {
            gameManager.UI.PanelComplete.SetActive(true);
            gameManager.UI.Leave.onClick.AddListener(() => PhotonNetwork.LeaveRoom(false));
        }
    }
}
