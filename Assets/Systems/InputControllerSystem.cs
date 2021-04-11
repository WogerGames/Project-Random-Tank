using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class InputControllerSystem : IEcsRunSystem
{
    readonly Joystick joystick;

    readonly EcsFilter<PLayerComponent, MoveComponent>.Exclude<AIControllerComponent> players;

    void IEcsRunSystem.Run()
    {
        foreach (var p in players)
        {
            var view = players.Get1(p).view;
            ref var move = ref players.Get2(p);

            if (!view.GetComponent<PhotonView>()) continue;

            if (view.GetComponent<PhotonView>().IsMine)
            {

                if (joystick.Direction == Vector2.zero)
                {
                    var x = Input.GetAxis("Horizontal");
                    var y = Input.GetAxis("Vertical");
                    move.Value = new Vector2(x, y) * Time.deltaTime * move.Speed;
                }
                else
                {
                    move.Value = joystick.Direction * Time.deltaTime * move.Speed;
                }


            }
        }
    }
}
