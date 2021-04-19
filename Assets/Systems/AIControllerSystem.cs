using Leopotam.Ecs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

sealed class AIControllerSystem : IEcsRunSystem
{
    // auto-injected fields.
    readonly EcsWorld _world = null;

    readonly EcsFilter<PlayerComponent, MoveComponent, AIControllerComponent> players;
    readonly EcsFilter<SpawnComponent> spawns;
    readonly EcsFilter<PlayerComponent> allPlayers;

    void IEcsRunSystem.Run()
    {
        if (!MultiplayerManager.IsMaster)
            return;

        // По очереди распределяем задачи между ботами
        foreach (var p in players)
        {
            ref var player = ref players.Get1(p);
            ref var ai = ref players.Get3(p);
            var view = player.view;
            
            if(ai.timerToCheckState < 0.5f)
            {
                ai.timerToCheckState += Time.deltaTime;
            }
            else
            {
                // Ищем всех врагов
                if(CheckOverviewArea(player, out var enemies))
                {
                    players.Get2(p).Value = Vector2.zero;
                    var enemyNext = GetNextEnemy(player, enemies);
                    ai.enemyTarget = enemyNext.Get<PlayerComponent>().view.transform;
                    ai.moveTarget = Vector2.zero;
                    ai.timerToCheckState = 0;
                }
                else // Если не находим врага, идем к вражескому спауну
                {
                    //Debug.Log("------------------------------------------------");
                    var spawn = GetNextEnemySpawn(player);
                    ai.moveTarget = spawn.pos;
                    ai.timerToCheckState = 0;
                    ai.enemyTarget = null;
                }
            }

            
        }
    }

    private bool CheckOverviewArea(PlayerComponent player, out List<EcsEntity> enemies)
    {
        enemies = new List<EcsEntity>();

        var hits = Physics2D.CircleCastAll(player.view.transform.position, 10, Vector2.zero);

        if(hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                var enemyView = hit.transform.GetComponent<Player>();
                if (enemyView)
                {
                    foreach (var p in allPlayers)
                    {
                        ref var enemy = ref allPlayers.Get1(p);
                        //Debug.Log($"{player.teamNum} - {enemy.teamNum} + {hits.Length}");
                        if(enemy.view == enemyView && enemy.teamNum != player.teamNum)
                        {
                            enemies.Add(allPlayers.GetEntity(p));
                        }
                    }
                }
            }
            if(enemies.Count > 0)
                return true;
        }

        return false;
    }

    SpawnComponent GetNextEnemySpawn(PlayerComponent player)
    {
        SpawnComponent result = default;
        float minDistance = float.MaxValue;

        foreach (var s in spawns)
        {
            ref var spawn = ref spawns.Get1(s);

            if (spawn.spawnType == teams[player.teamNum])
                continue;

            var distance = Vector2.Distance(spawn.pos, player.view.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                result = spawn;
            }
        }

        return result;
    }

    EcsEntity GetNextEnemy(PlayerComponent player, List<EcsEntity> enemies)
    {
        EcsEntity result = default;
        float minDistance = float.MaxValue;
        foreach (var enemy in enemies)
        {
            var p1 = player.view.transform.position;
            var p2 = enemy.Get<PlayerComponent>().view.transform.position;
            var distance = Vector2.Distance(p1, p2);
            if(distance < minDistance)
            {
                minDistance = distance;
                result = enemy;
            }
        }

        return result;
    }

    readonly Dictionary<TeamNum, SpawnType> teams = new Dictionary<TeamNum, SpawnType>
    {
        { TeamNum.One, SpawnType.Command_1 },
        { TeamNum.Two, SpawnType.Command_2 }
    };
}
