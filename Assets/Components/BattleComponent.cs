using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;

public struct BattleComponent
{
    public List<EcsEntity> teamOne;
    public List<EcsEntity> teamTwo;
}


public enum TeamNum
{
    One,
    Two
}