using Leopotam.Ecs;
using UnityEngine;

sealed class CrossCaptureSystem : IEcsRunSystem
{
    readonly EcsFilter<SpawnComponent, CrossCaptureTag> spawns;

    float time = 0.3f;
    bool reset = true;
    Color curColor = Color.white;

    void IEcsRunSystem.Run()
    {
        foreach (var s in spawns)
        {
            var spawn = spawns.Get1(s).view;

            if (reset)
            {
                reset = false;
                LeanTween.value(spawn.gameObject, t => { Signal(spawn, t); }, 0, 1, time).setEaseOutQuad()
                    .setOnComplete
                (
                    () => 
                    {
                        if (curColor == spawn.commandColor_1)
                        {
                            curColor = spawn.commandColor_2;
                        }
                        else
                        {
                            curColor = spawn.commandColor_1;
                        }

                        if (spawns.GetEntitiesCount() > 0)
                        {
                            LeanTween.value(spawn.gameObject, t => { Signal(spawn, t); }, 1, 0, time).setEaseOutQuad()
                            .setOnComplete(Reset);
                        }
                        else
                        {
                            Reset();
                        }
                    }
                );
            }
        }
    }

    void Signal(SpawnPoint spawn, float t)
    {
        if (spawns.GetEntitiesCount() == 0)
            return;

        var col = Color.Lerp(curColor, Color.white, t);

        spawn.border.color = col;
        spawn.inner.color = col;  
    }

    void Reset() => reset = true;
    
}
