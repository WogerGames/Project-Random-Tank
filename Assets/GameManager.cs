using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerAIPrefab;

    public Joystick rotationJoystick;

    public static GameManager Instance;

    public EcsWorld EcsWorld
    {
        get
        {
            return FindObjectOfType<Godlike>()?.EcsWorld;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LeanTween.delayedCall(1, SpawnAI);
    }

    void SpawnAI()
    {
        StartCoroutine(Spawn());

        IEnumerator Spawn()
        {
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(.01f);

                if (MultiplayerManager.IsMaster)
                {
                    var ai = MultiplayerManager.Spawn(playerAIPrefab, playerAIPrefab.transform.position);
                }
            }
        }

        
    }
}
