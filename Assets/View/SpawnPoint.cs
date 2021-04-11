using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] public SpriteRenderer border;
    [SerializeField] public SpriteRenderer inner;
    [SerializeField] public Color commandColor_1;
    [SerializeField] public Color commandColor_2;

    public SpriteRenderer SpriteRenderer => spriteRenderer;

    [Space]

    public float captureValue;

    //Dictionary<SpawnType, Color> colors = new Dictionary<SpawnType, Color32> 
    //{
    //    {SpawnType.Command_1, commandColor_2}
    //};
    public Color32 GetCurColor(SpawnType spawnType)
    {
        if (spawnType == SpawnType.Command_2)
            return commandColor_2;

        if (spawnType == SpawnType.Command_1)
            return commandColor_1;

        return Color.white;
    }

    public Color32 GetNewColor(SpawnType spawnType)
    {
        if (spawnType == SpawnType.Command_1)
            return commandColor_2;

        if (spawnType == SpawnType.Command_2)
            return commandColor_1;

        return Color.white;
    }
}
