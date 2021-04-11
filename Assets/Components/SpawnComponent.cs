using UnityEngine;

public struct SpawnComponent
{
    public SpawnType spawnType;
    public Vector2 pos;
    public float captureValue;
    public SpawnPoint view;
    public Color lastCaptureColor;
}

public enum SpawnType
{
    Command_1,
    Command_2,
    Neutral,
}
