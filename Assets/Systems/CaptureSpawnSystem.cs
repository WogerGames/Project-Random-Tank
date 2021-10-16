using Leopotam.Ecs;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


sealed class CaptureSpawnSystem : IEcsRunSystem
{
    readonly EcsFilter<PlayerComponent> players;
    readonly EcsFilter<SpawnComponent> spawns;

    readonly Dictionary<TeamNum, SpawnType> teams = new Dictionary<TeamNum, SpawnType>
    {
        { TeamNum.One, SpawnType.Command_1 },
        { TeamNum.Two, SpawnType.Command_2 }
    };

    float captureSpeed = 0.18f;

    void IEcsRunSystem.Run()
    {
        foreach (var s in spawns)
        {
            ref var spawn = ref spawns.Get1(s);

            var hits = Physics.SphereCastAll(new Vector3(spawn.pos.x, 0, spawn.pos.y), 1.8f, Vector3.up);
            // Отфильтровываем только игроков
            hits = hits.Where(h => h.collider.GetComponent<Player>()).ToArray();
                
            bool isIntersection = false;

            //===========================================
            spawn.view.captureValue = spawn.captureValue;
            //===========================================

            List<TeamNum> intersectTeams = new List<TeamNum>();

            int idx = 0;

            foreach (var hit in hits)
            {
                idx++;

                if (!hit.transform.GetComponent<Player>())
                    continue;
                
                isIntersection = true;

                //Находим сущности игроков исходя из имеющихся view игроков в пересечении
                foreach (var p in players)
                {
                    ref var player = ref players.Get1(p);

                    var playerIsFound = hit.transform.GetComponent<Player>() == player.view;

                    if (playerIsFound)
                    {
                        intersectTeams.Add(player.teamNum);
                        intersectTeams = intersectTeams.Distinct().ToList();
                    }
                }

                if(idx < hits.Length)
                {
                    continue;
                }

                foreach (var p in players)
                {
                    ref var player = ref players.Get1(p);

                    var playerIsFound = hit.transform.GetComponent<Player>() == player.view;
                   
                    if (playerIsFound)
                    {
                        //Debug.Log($"{teams[player.teamNum]} -+-+-- Тип спауна {spawn.spawnType} -+-+-+ Тима {player.teamNum}");
                        //Debug.Log($"{intersectTeams.Count} - каунт хуянт {idx}");
                        ref var spawnEntity = ref spawns.GetEntity(s);
                        
                        if (spawnEntity.Has<CrossCaptureTag>())
                        {
                            if(intersectTeams.Count > 1)
                            {
                                continue;
                            }
                            else
                            {
                                spawnEntity.Del<CrossCaptureTag>();
                                var col = spawn.view.GetCurColor(spawn.spawnType);
                                ChangeColor(ref spawn, col, col);
                            }
                        }

                        if(intersectTeams.Count > 1)
                        {
                            spawnEntity.Get<CrossCaptureTag>();
                            continue;
                        }

                        if (teams[player.teamNum] == spawn.spawnType)
                        {
                            //Debug.Log($"{teams[player.teamNum]} ^^^^ {spawn.spawnType}");
                            if (spawn.captureValue < 1)
                            {
                                spawn.captureValue += Time.deltaTime * captureSpeed;
                                Color32 cur = Color.white;
                                Color32 tar = spawn.view.GetCurColor(teams[player.teamNum]);

                                ChangeColor(ref spawn, cur, tar);

                                spawn.lastCaptureColor = spawn.view.GetCurColor(teams[player.teamNum]);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        //Debug.Log($"{teams[player.teamNum]} -+-+-- {spawn.spawnType}");
                        if (spawn.captureValue > 0 && spawn.spawnType != SpawnType.Neutral && teams[player.teamNum] != spawn.spawnType)
                        {
                            spawn.captureValue -= Time.deltaTime * captureSpeed;
                            Color32 cur = spawn.view.GetCurColor(spawn.spawnType);
                            Color32 tar = Color.white;

                            ChangeColor(ref spawn, tar, cur);
                        }

                        if (spawn.captureValue <= 0 && spawn.spawnType != SpawnType.Neutral)
                        {
                            spawn.spawnType = SpawnType.Neutral;
                            spawn.lastCaptureColor = spawn.view.GetCurColor(teams[player.teamNum]);
                        }

                        if (spawn.captureValue < 1 && spawn.spawnType == SpawnType.Neutral)
                        {
                            if (spawn.lastCaptureColor == Color.white
                                || spawn.lastCaptureColor == spawn.view.GetCurColor(teams[player.teamNum]))
                            {
                                spawn.captureValue += Time.deltaTime * captureSpeed;
                                Color32 cur = Color.white;
                                Color32 tar = spawn.view.GetCurColor(teams[player.teamNum]);

                                ChangeColor(ref spawn, cur, tar);

                                spawn.lastCaptureColor = spawn.view.GetCurColor(teams[player.teamNum]);

                                if (spawn.captureValue >= 1 && teams[player.teamNum] != spawn.spawnType)
                                {
                                    spawn.spawnType = teams[player.teamNum];
                                    //spawn.lastCaptureColor = spawn.view.GetCurColor(teams[player.teamNum]);
                                }
                            }
                            else
                            {
                                spawn.captureValue -= Time.deltaTime * captureSpeed;
                                Color32 cur = Color.white;
                                Color32 tar = spawn.lastCaptureColor;//view.GetCurColor(teams[player.teamNum]);

                                ChangeColor(ref spawn, cur, tar);
                                if (spawn.captureValue <= 0) spawn.lastCaptureColor = Color.white;
                            }

                            
                        }

                        
                    }
                }
            }
            // Если нету игроков в точке спауна
            if (!isIntersection)
            {
                if (spawn.captureValue < 1 && spawn.spawnType != SpawnType.Neutral)
                {
                    spawn.captureValue += Time.deltaTime * captureSpeed;
                    Color32 cur = Color.white;
                    Color32 tar = spawn.view.GetCurColor(spawn.spawnType);

                    ChangeColor(ref spawn, cur, tar);
                }

                if (spawn.captureValue > 0 && spawn.spawnType == SpawnType.Neutral)
                {
                    spawn.captureValue -= Time.deltaTime * captureSpeed;
                    Color32 cur = Color.white;
                    Color32 tar = spawn.lastCaptureColor;

                    ChangeColor(ref spawn, cur, tar);
                }

                if(spawn.captureValue <= 0 && spawn.spawnType == SpawnType.Neutral)
                {
                    spawn.lastCaptureColor = Color.white;
                }
                
            }
        }

    }

    void ChangeColor(ref SpawnComponent spawn, Color tar, Color cur)
    {
        Color32 col = Color.Lerp(tar, cur, spawn.captureValue);

        spawn.view.border.color = col;
        spawn.view.inner.color = col;

    }
}
