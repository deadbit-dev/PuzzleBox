using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class Trajectory : MonoBehaviour
    {
        public float Length = 1f;
        public int SegmentsCount = 30;
        public GameObject SegmentPrefab;
    }
    
    public struct TrajectoryCmp
    {
        public float Length;
        public int SegmentCount;
        public GameObject SegmentPrefab;
    }

    public struct SegmentBuffer
    {
        public GameObject[] Value;
    }
    
    public class TrajectoryLoadSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent, LevelCmp>> loadLevelEventFilter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<ShotPointCmp>> shotPointFilter = Worlds.PresentationWorld;
        
        private readonly EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LinkedToShotPointCmp> linkedToShotPointCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TrajectoryCmp> trajectoryCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<SegmentBuffer> segmentBufferPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var levelEntity in loadLevelEventFilter.Value)
            {
                foreach (var levelObject in loadLevelEventFilter.Pools.Inc2.Get(levelEntity).Level.LevelObjects)
                {
                    if (levelObject.GetType() != typeof(ShotPoint)) continue;
                    var shotPointView = (ShotPoint)levelObject;

                    var trajectory = shotPointView.Trajectory;

                    var entity = world.Value.NewEntity();

                    positionCmpPool.Value.Add(entity);
                    transformRefCmpPool.Value.Add(entity).Value = trajectory.transform;
                    
                    ref var trajectoryCmp = ref trajectoryCmpPool.Value.Add(entity);
                    trajectoryCmp.Length = trajectory.Length;
                    trajectoryCmp.SegmentCount = trajectory.SegmentsCount;
                    trajectoryCmp.SegmentPrefab = trajectory.SegmentPrefab;

                    segmentBufferPool.Value.Add(entity);
                    
                    if (FindShotPointEntityByTransform(shotPointView.transform, out var shotPointEntity))
                    {
                        linkedToShotPointCmpPool.Value.Add(entity).Value = world.Value.PackEntity(shotPointEntity);
                    }

                    levelElementCmpPool.Value.Add(entity).LevelIndex = loadLevelEventFilter.Pools.Inc1.Get(levelEntity).LevelIndex;
                }
            }
        }

        private bool FindShotPointEntityByTransform(Transform transform, out int entity)
        {
            entity = -1;
            foreach (var shotPointEntity in shotPointFilter.Value)
            {
                ref var transformRefCmp = ref transformRefCmpPool.Value.Get(shotPointEntity);
                if (transform != transformRefCmp.Value) continue;

                entity = shotPointEntity;
                break;
            }

            return entity != -1;
        }
    }
    
    public class TrajectorySystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<TransformRefCmp, TrajectoryCmp, SegmentBuffer, LinkedToShotPointCmp>> filter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<AimingTag> aimingTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<AttachedProjectile> attachedProjectilePool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ProjectileCmp> projectileCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ShotPointCmp> shotPointCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var trajectoryEntity in filter.Value)
            {
                ref var linkedToShotPointCmp = ref filter.Pools.Inc4.Get(trajectoryEntity); 
                if (!linkedToShotPointCmp.Value.Unpack(world.Value, out var shotPointEntity)) continue;
                if (aimingTagPool.Value.Has(shotPointEntity)) SimulateTrajectory(trajectoryEntity, shotPointEntity);
            }
        }

        private void SimulateTrajectory(int trajectoryEntity, int shotPointEntity)
        {
            ref var shotPointCmp = ref shotPointCmpPool.Value.Get(shotPointEntity);
            ref var attachedProjectile = ref attachedProjectilePool.Value.Get(shotPointEntity);

            if (!attachedProjectile.Value.Unpack(world.Value, out var projectileEntity)) return;
            
            ref var trajectoryTransformRefCmp = ref filter.Pools.Inc1.Get(trajectoryEntity);
            ref var trajectoryCmp = ref filter.Pools.Inc2.Get(trajectoryEntity);
            ref var segmentBuffer = ref filter.Pools.Inc3.Get(trajectoryEntity);
            ref var projectileTransformRefCmp = ref transformRefCmpPool.Value.Get(projectileEntity);
            ref var projectileCmp = ref projectileCmpPool.Value.Get(projectileEntity);
            
            var velocity = shotPointCmp.CurrentForce / projectileCmp.Rigidbody2D.mass;
            var timeStep = trajectoryCmp.Length / trajectoryCmp.SegmentCount;
            
            segmentBuffer.Value ??= new GameObject[trajectoryCmp.SegmentCount];
            for (var i = 0; i < trajectoryCmp.SegmentCount; i++)
            {
                var deltaVelocity = CalculateDeltaVelocity(velocity, projectileCmp.Rigidbody2D.gravityScale, projectileCmp.Rigidbody2D.drag, timeStep, i);
                var nextDeltaVelocity = CalculateDeltaVelocity(velocity, projectileCmp.Rigidbody2D.gravityScale, projectileCmp.Rigidbody2D.drag, timeStep, i + 1);
                var delta = nextDeltaVelocity - deltaVelocity;

                if (segmentBuffer.Value[i] != null) Object.Destroy(segmentBuffer.Value[i]);

                segmentBuffer.Value[i] = Object.Instantiate(
                    trajectoryCmp.SegmentPrefab,
                    projectileTransformRefCmp.Value.position + (Vector3)deltaVelocity,
                    Quaternion.Euler(new Vector3(0,0, 90 - Mathf.Atan2(delta.normalized.x, delta.normalized.y) * Mathf.Rad2Deg)),
                    trajectoryTransformRefCmp.Value);
            }
        }

        private static Vector2 CalculateDeltaVelocity(Vector2 velocity, float gravityScale, float drag, float timeStep, int i)
        {
            var timeOffset = timeStep * i;
            var progress = velocity * timeOffset;
            progress.x -= drag;
            progress.y -= (-0.5f * (Physics2D.gravity.y * gravityScale) * timeOffset * timeOffset) - drag;
            return progress;
        }
    }
}