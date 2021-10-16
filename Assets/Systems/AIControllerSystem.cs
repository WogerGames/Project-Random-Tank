using Leopotam.Ecs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


sealed class AIControllerSystem : IEcsRunSystem
{
    // auto-injected fields.
    readonly EcsWorld _world = null;

    //readonly EcsFilter<PlayerComponent, MoveComponent, AIControllerComponent> players;
    readonly EcsFilter<PlayerComponent, AIControllerComponent> players;
    readonly EcsFilter<SpawnComponent> spawns;
    readonly EcsFilter<PlayerComponent> allPlayers;

    bool katkaIsCompleted = false;

    void IEcsRunSystem.Run()
    {
        if (katkaIsCompleted)
            return;

        if (!MultiplayerManager.IsMaster)
            return;


        // По очереди распределяем задачи между ботами
        foreach (var p in players)
        {
            ref var player = ref players.Get1(p);
            ref var ai = ref players.Get2(p);
            var view = player.view;
            
            if(ai.timerToCheckState < 0.5f)
            {
                ai.timerToCheckState += Time.deltaTime;
            }
            else
            {

                ai.timerToCheckState = 0;

                // Ищем всех врагов
                if (CheckOverviewArea(player, out var enemies))
                {
                    //players.Get2(p).Value = Vector2.zero;
                    var enemyNext = GetNextEnemy(player, enemies);
                    ai.enemyTarget = enemyNext.Get<PlayerComponent>().view.transform;
                    ai.moveTarget = Vector2.zero;
                    

                    //Debug.Log("Есть враг в поле зрения, стоим ебашим по нему");
                }
                else // Если не находим врага, идем к вражескому спауну
                {
                    //Debug.Log("Врагов не видать");
                    if (FindNextEnemySpawn(player, out var spawn))
                    {
                        ai.moveTarget = spawn.pos;
                        ai.enemyTarget = null;
                        //Debug.Log("Пиздую к спауну");
                    }
                    // НАДО ПЕРЕПИСАТЬ TODO
                    else if(!katkaIsCompleted)
                    {
                        katkaIsCompleted = true;
                        _world.NewEntity().Get<CompleteGameEvent>().winningTeam = player.teamNum;
                    }
                }
            }

            
        }
    }

    private bool CheckOverviewArea(PlayerComponent player, out List<EcsEntity> enemies)
    {
        enemies = new List<EcsEntity>();

        //var hits = Physics2D.CircleCastAll(player.view.transform.position, 10, Vector2.zero);
        var hits = Physics.SphereCastAll(player.view.transform.position, 7, Vector3.up);
        hits = hits.Where(h => h.collider.GetComponent<Player>()).ToArray();

        if (hits.Length > 0)
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
                            if (CheckPresenceObstacles(player, enemy))
                                continue;

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

    bool FindNextEnemySpawn(PlayerComponent player, out SpawnComponent spawnComponent)
    {
        spawnComponent = default;
        bool result = false;
        float minDistance = float.MaxValue;

        foreach (var s in spawns)
        {
            ref var spawn = ref spawns.Get1(s);

            if (spawn.spawnType == teams[player.teamNum])
                continue;

            var playerPos2D = new Vector2(player.view.transform.position.x, player.view.transform.position.z);
            var distance = Vector2.Distance(spawn.pos, playerPos2D);

            if (distance < minDistance)
            {
                minDistance = distance;
                spawnComponent = spawn;
                result = true;
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
            var distance = Vector3.Distance(p1, p2);
            if(distance < minDistance)
            {
                minDistance = distance;
                result = enemy;
            }
        }

        return result;
    }

    bool CheckPresenceObstacles(PlayerComponent player, PlayerComponent enemy)
    {
        var playerPos = player.view.transform.position;
        var enemyPos = enemy.view.transform.position;

        var dir = enemyPos - playerPos;

        var hits = Physics.RaycastAll(playerPos, dir, Vector3.Distance(playerPos, enemyPos));

        foreach (var hit in hits)
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("NavMesh"))
            {
                return true;
            }
        }


        return false;
    }

    readonly Dictionary<TeamNum, SpawnType> teams = new Dictionary<TeamNum, SpawnType>
    {
        { TeamNum.One, SpawnType.Command_1 },
        { TeamNum.Two, SpawnType.Command_2 }
    };
}
