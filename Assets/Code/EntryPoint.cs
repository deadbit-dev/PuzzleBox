using UnityEngine;
using UnityEngine.SceneManagement;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;

namespace JoyTeam
{
    internal class EntryPoint : MonoBehaviour
    {
        [SerializeField] private SceneData SceneData;
        [SerializeField] private StaticData StaticData;

        private RuntimeData runtimeData;

        private Scene scene;
        private EcsWorld world;
        private IEcsSystems systems;

        private Scene physicsScene;
        private EcsWorld physicsWorld;
        private IEcsSystems physicsSystems;

#if UNITY_EDITOR
        private EcsWorld editorWorld;
        private IEcsSystems editorSystems;

        private void OnValidate()
        {
            editorWorld = new EcsWorld();
            editorSystems = new EcsSystems(editorWorld);
            editorSystems
                .Init();
        }

        private void OnGUI() => editorSystems?.Run();
        
        //TODO: SOMEWHERE DESTROY SYSTEMS AND WORLD OF EDITOR
#endif

        private void Start()
        {
            runtimeData = new RuntimeData();
            
            runtimeData.Scene = SceneManager.GetActiveScene();
            runtimeData.PhysicsScene = SceneManager.GetActiveScene().GetPhysicsScene2D();

            var parameters = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
            runtimeData.PredictionScene = SceneManager.CreateScene("Prediction", parameters);
            runtimeData.PredictionPhysicsScene = runtimeData.PredictionScene.GetPhysicsScene2D();
            
            world = new EcsWorld();
            physicsWorld = new EcsWorld();
            
            systems = new EcsSystems(world);
            systems
                .AddWorld(world, Worlds.PresentationWorld)
                .AddWorld(physicsWorld, Worlds.PhysicsWorld)
                .Add(new InitializeSystem())
                .Add(new RestartLevelButtonSystem())
                .Add(new RestartLevelSystem())
                .Add(new UnloadLevelSystem())
                .Add(new LoadLevelSystem())
#region LOAD SYSTEMS   
                .Add(new LoadRestartLevelButtonSystem())
                .Add(new LoadShotPointSystem())
                .Add(new LoadRotationHandlerSystem())
                .Add(new TrajectoryLoadSystem())
                .Add(new LoadBarSystem())
                .Add(new LoadSlotSystem())
                .Add(new LoadProjectileSystem())
                .Add(new LoadBounceZoneSystem())
                .Add(new LoadConnectionPointSystem())
#endregion
                .Add(new UpdatePositionFromTransform())
                .Add(new ProjectileOnDownSystem())
                .Add(new ProjectileOnDragSystem())
                .Add(new ProjectileOnUpSystem())
                .Add(new ShotPointOnDownSystem())
                .Add(new ShotPointOnDragSystem())
                .Add(new ShotPointOnUpSystem())
                .Add(new RotationHandlerActivationSystem())
                .Add(new RotationHandlerOnDownSystem())
                .Add(new RotationHandlerOnDraggSystem())
                .Add(new RotationHandlerOnUpSystem())
                .Add(new TrajectorySystem())
                .Add(new BarActivationSystem())
                .Add(new ConnectionPointSystem())
                .Add(new PositionTweenSystem())
                .Add(new UpdateProjectilePositionInSlotSystem())
                .Add(new UpdateTransformFromPosition())
#region ONE FRAME COMPONENTS - EVENTS
                .DelHere<ClickEvent>()
                .DelHere<LoadLevelEvent>()
                .DelHere<UnloadLevelEvent>()
                .DelHere<RestartLevelEvent>()
                .DelHere<PositionTweenEndEvent>()
#endregion
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Inject(StaticData, SceneData, runtimeData)
                .Init();
            
            
            physicsSystems = new EcsSystems(physicsWorld);
            physicsSystems
                .AddWorld(physicsWorld, Worlds.PhysicsWorld)
                .AddWorld(world, Worlds.PresentationWorld)
                .Add(new BounceZoneSystem())
                #region ONE FRAME COMPONENTS - EVENTS
                .DelHere<CollisionEnterEvent>()
                #endregion
                .Inject(StaticData, SceneData, runtimeData)
                .Init();
        }

        private void Update() => systems?.Run();
        private void FixedUpdate() => physicsSystems?.Run();

        private void OnDestroy()
        {
            if (physicsSystems != null)
            {
                physicsSystems.Destroy();
                physicsSystems = null;
            }

            if (physicsWorld != null)
            {
                physicsWorld.Destroy();
                physicsWorld = null;
            }
            
            if (systems != null)
            {
                systems.Destroy();
                systems = null;
            }

            if (world != null)
            {
                world.Destroy();
                world = null;
            }
        }
    }
}