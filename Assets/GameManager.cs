using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

public class GameManager : MonoBehaviour
{
    [SerializeField] int countAI = 3;
    [SerializeField] float delayBetweenSpawn = .3f;
    [SerializeField] GameObject playerAIPrefab;
    [SerializeField] UI ui;

    public Joystick rotationJoystick;

    public static GameManager Instance;

    private EcsWorld ecsWorld;

    public EcsWorld EcsWorld
    {
        get
        {
            if(ecsWorld == null)
            {
                ecsWorld = FindObjectOfType<Godlike>()?.EcsWorld;
            }
            return ecsWorld;
        }
    }

    public UI UI => ui;

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
            for (int i = 0; i < countAI; i++)
            {
                yield return new WaitForSeconds(delayBetweenSpawn);

                if (MultiplayerManager.IsMaster)
                {
                    var ai = MultiplayerManager.Spawn(playerAIPrefab, playerAIPrefab.transform.position);
                }
            }
        }

        
    }
}
