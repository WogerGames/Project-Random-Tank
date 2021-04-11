using Leopotam.Ecs;
using UnityEngine;

sealed class SpawnInitSystem : IEcsInitSystem
{
    readonly EcsFilter<SpawnComponent> spawns;

    public void Init()
    {
        
        foreach (var s in spawns)
        {
            ref var spawn = ref spawns.Get1(s);
            
            Color32 color;

            if (spawn.spawnType == SpawnType.Command_1)
            {
                color = spawn.view.commandColor_1;
            }
            else
            {
                color = spawn.view.commandColor_2;
            }

            spawn.view.border.color = color;
            spawn.view.inner.color  = color;
            spawn.captureValue = 1;
        }
    }
}
