using Leopotam.Ecs;
using UnityEngine;

public interface IPerk { }

public static class PerkExtension
{
    public static EventCode AddPerkToEntity(this IPerk perk, ref EcsEntity entity)
    {
        if(perk is Perk1)
        {
            entity.Get<Perk1>();
            return EventCode.Perk1;
        }
        if(perk is Perk2)
        {
            entity.Get<Perk2>();
            return EventCode.Perk2;
        }
        if(perk is Perk3)
        {
            entity.Get<Perk3>();
            return EventCode.Perk3;
        }

        Debug.LogError($"Нет правила для перка {perk}");
        return default;
    }

    public static IPerk GetPerk(this EventCode eventCode)
    {
        if(eventCode == EventCode.Perk1)
        {
            return new Perk1();
        }
        if (eventCode == EventCode.Perk2)
        {
            return new Perk2();
        }
        if (eventCode == EventCode.Perk3)
        {
            return new Perk3();
        }

        Debug.LogError($"Нет правила для перка {eventCode}");
        return default;
    }
}
