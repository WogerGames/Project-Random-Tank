using Leopotam.Ecs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

sealed class PlayerSpawnSystems : IEcsRunSystem
{
    readonly EcsFilter<PlayerSpawnEvent> events;
    readonly EcsFilter<PLayerComponent, HealthPointComponent> playersFilter;
    readonly EcsFilter<BattleComponent> battleFilter;
    readonly EcsFilter<SpawnComponent> spawnsFilter;

    readonly List<SpawnComponent> spawns = new List<SpawnComponent>();
    readonly List<PLayerComponent> players = new List<PLayerComponent>();

    int playerHp = 30;

    void IEcsRunSystem.Run()
    {
        foreach (var i in spawnsFilter) spawns.Add(spawnsFilter.Get1(i));
        foreach (var i in playersFilter) players.Add(playersFilter.Get1(i));
        //Debug.Log(events.GetEntitiesCount());
        // Это необходимо для правильной очередности 
        // появления игроков, если мы подключаемся к уже созданной игре
        List<Player> photonPlayers = new List<Player>();
        foreach (var e in events) photonPlayers.Add(events.Get1(e).player);
        //photonPlayers.ForEach(p => Debug.Log(p.photonView.OwnerActorNr + " -***"));
        photonPlayers = photonPlayers.OrderBy(p => p.photonView.OwnerActorNr).ToList();
        //photonPlayers.ForEach(p => Debug.Log(p.photonView.OwnerActorNr + " -+++"));

        foreach (var e in photonPlayers)
        {
            Debug.Log($"{PhotonNetwork.PlayerList.Length % 2} ...");
           
            var spawned = e;

            var player = players.Find(p => p.view == spawned);
           
            EcsEntity entity = GetPlayerEntity(player);
            Debug.Log(GetPlayerEntity(player));
            entity.Get<HealthPointComponent>().Value = playerHp;

            foreach (var b in battleFilter)
            {
                ref var battle = ref battleFilter.Get1(b);

                SpawnComponent spawn = default;

                // Получение порядкового номера
                var id = int.Parse(e.photonView.ViewID.ToString().Substring(2));
                //Debug.Log(id + " ==-=--=--=-=");

                if (id % 2 > 0)
                {
                    spawn = spawns.Find(s => s.spawnType == SpawnType.Command_1);
                    battle.teamOne.Add(entity);
                    entity.Get<PLayerComponent>().teamNum = TeamNum.One;
                    player.teamNum = TeamNum.One;
                }
                else
                {
                    spawn = spawns.Find(s => s.spawnType == SpawnType.Command_2);
                    battle.teamTwo.Add(entity);
                    entity.Get<PLayerComponent>().teamNum = TeamNum.Two;
                    player.teamNum = TeamNum.Two;
                }
                

                player.view.collider.enabled = true;
                player.view.OnDamage(playerHp);

                if (player.view.photonView.IsMine)
                {
                    player.view.transform.position = spawn.pos + GetRandomOffset();
                }

                Debug.Log($"тима двэ {battle.teamTwo.Count} || тима одын {battle.teamOne.Count} поз хуёз {player.view.transform.position}");
                
            }

            
        }
    }

    Vector2 GetRandomOffset()
    {
        float angle = Random.Range(0, 36) * 10;
        float x = Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = Mathf.Cos(Mathf.Deg2Rad * angle);
        return new Vector2(x, y) * 1.5f;
    }

    EcsEntity GetPlayerEntity(PLayerComponent player)
    {
        foreach (var i in playersFilter)
        {
            if(playersFilter.Get1(i).view == player.view)
            {
                return playersFilter.GetEntity(i);
            }
        }
        return default;
    }
}
