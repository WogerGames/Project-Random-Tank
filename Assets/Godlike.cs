using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

[DefaultExecutionOrder(350)]
public class Godlike : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Joystick joystick;

    private PlayerManager playerManager;

    private EcsWorld ecsWorld;
    private EcsSystems systems;
    private EcsSystems systemsPhysics;
    private System.Random random;

    public EcsWorld EcsWorld => ecsWorld;
    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        ecsWorld = new EcsWorld();
        systems = new EcsSystems(ecsWorld);
        systemsPhysics = new EcsSystems(ecsWorld);
        random = new System.Random();

        playerManager = FindObjectOfType<PlayerManager>();

#if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(ecsWorld);
        Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(systems);
#endif

        systems
            .Add(new InputControllerSystem())
            .Add(new MovementSystem())
            .Add(new RotationSystem())
            .Add(new CooldownRateSystem())
            .Add(new FireSystem())
            .Add(new ProjectileMoveSystem())
            .Add(new IncreaseSystem())
            .Add(new DamageSystem())
            .Add(new HpShowSystem())
            .Add(new PhotonHealthPointSystem())
            .Add(new SpawnPlayerSystems())
            .Add(new DestroySystem())
            .Add(new CaptureSpawnSystem())
            .Add(new SpawnInitSystem())
            .Add(new CrossCaptureSystem())
            .Add(new ProjectileLifetimeSystem())
            .Add(new DestroyedCounter())
            .Add(new PerkInitSystem())
            .Add(new ProgressSystem())
            .Add(new PerkSystem())

            .Add(new AIControllerSystem())
            .Add(new AIMoveDirectionSystem())
            .Add(new AIRotationSystem())
            .Add(new AIFireSystem())
            .Add(new AIPerkSystem())

            .Add(new Perk1System())
            .Add(new Perk2System())
            .Add(new Perk3System())

            .Add(new CompleteGameSystem())
            .Add(new UICompleteGameSystem())
            .Add(new PlayerDataSystem())// TEmporary solutione áëýò
            .Add(new IncreaseUISystem())
            .Add(new PhotonIncreaseInitSystem())
            .Add(new PhotonIncreaseSystem())


            .OneFrame<PhotonHealthPointEvent>()
            .OneFrame<ProjectileCollisionEvent>()
            .OneFrame<PlayerSpawnEvent>()
            .OneFrame<DestroyEvent>()
            .OneFrame<ProgressEvent>()
            .OneFrame<CompleteGameEvent>()
            .OneFrame<IncreasedEvent>()

            .OneFrame<Perk1>()
            .OneFrame<Perk2>()
            .OneFrame<Perk3>()

            
            .Inject(gameManager)
            .Inject(joystick);


        systemsPhysics
            //.Add(new CarAccelerationSystem())
            //.Add(new CarDecelerationSystem())
            .Inject(gameManager);

        if (playerManager)
        {
            systems.Inject(playerManager);
        }

        systems.ProcessInjects();
        systemsPhysics.ProcessInjects();

        
    }

    void Start()
    {
        systems.Init();
        systemsPhysics.Init();

        CreateBattleComponent();
    }

    void CreateBattleComponent()
    {
        ref var battle = ref ecsWorld.NewEntity().Get<BattleComponent>();
        battle.teamOne = new List<EcsEntity>();
        battle.teamTwo = new List<EcsEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        systems?.Run();
    }

    private void FixedUpdate()
    {
        systemsPhysics?.Run();
    }

    

    private void OnDisable()
    {
        if (systems == null)
            return;
        systems.Destroy();
        systems = null;

        ecsWorld.Destroy();
        ecsWorld = null;
    }
}
