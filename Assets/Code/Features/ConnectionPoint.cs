using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class ConnectionPoint : PresenterBase
    {
        public Joint2D[] Joints;
        [Range(0f, 1f)] public float ConnectionError;
    }
    
    public struct ConnectionPointCmp
    {
        public Joint2D[] Joints;
        public float ConnectionError;
    }

    public class LoadConnectionPointSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent, LevelCmp>> filter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ConnectionPointCmp> connectionPointCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var levelEntity in filter.Value)
            {
                foreach (var levelObject in filter.Pools.Inc2.Get(levelEntity).Level.LevelObjects)
                {
                    if (levelObject.GetType() != typeof(ConnectionPoint)) continue;
                    var view = (ConnectionPoint)levelObject;

                    var entity = world.Value.NewEntity();

                    transformRefCmpPool.Value.Add(entity).Value = view.transform;

                    ref var connectionPointCmp =
                        ref connectionPointCmpPool.Value.Add(entity);
                    connectionPointCmp.Joints = view.Joints;
                    connectionPointCmp.ConnectionError = view.ConnectionError;

                    levelElementCmpPool.Value.Add(entity).LevelIndex =
                        filter.Pools.Inc1.Get(levelEntity).LevelIndex;

                    view.Init(world.Value, world.Value.PackEntity(entity));
                }
            }
        }
    }
    
    public class ConnectionPointSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent>> loadLevelEventFilter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<ConnectionPointCmp>> connectionPointFilter = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            if (loadLevelEventFilter.Value.GetEntitiesCount() != 0)
            {
                foreach (var connectionPointEntity in connectionPointFilter.Value)
                    IgnoreCollision(ref connectionPointFilter.Pools.Inc1.Get(connectionPointEntity));
            }

            foreach (var connectionPointEntity in connectionPointFilter.Value)
            {
                ref var cmp = ref connectionPointFilter.Pools.Inc1.Get(connectionPointEntity);
                foreach (var joint in cmp.Joints)
                {
                    if (ValidateJointConnection((HingeJoint2D)joint, cmp.ConnectionError)) continue;

                    IgnoreCollision(ref cmp, false);

                    Object.Destroy(joint.gameObject);
                    world.Value.DelEntity(connectionPointEntity);
                }
            }
        }

        private static bool ValidateJointConnection(HingeJoint2D joint, float error)
        {
            var connectedPos = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
            var basePos = joint.attachedRigidbody.transform.position;
            return Math.CompareWithThreshold(connectedPos, basePos, error);
        }

        private static void IgnoreCollision(ref ConnectionPointCmp cmp, bool ignore = true)
        {
            if(cmp.Joints == null) return;
            for (var i = 0; i < cmp.Joints.Length; i++)
            {
                if (cmp.Joints[i] == null) continue;
                
                var currentBody = cmp.Joints[i].connectedBody;
                
                if (currentBody == null) continue;
                if (currentBody.attachedColliderCount == 0) continue;
                
                var currentColliders = new Collider2D[1];
                currentBody.GetAttachedColliders(currentColliders);
                
                for (var j = i + 1; j < cmp.Joints.Length; j++)
                {
                    if (cmp.Joints[i] == null) continue;
                    
                    var otherBody = cmp.Joints[j].connectedBody;
                    if (otherBody == null) continue;
                    if (otherBody.attachedColliderCount == 0) continue;
                    
                    var otherColliders = new Collider2D[1];
                    otherBody.GetAttachedColliders(otherColliders);
                    
                    Physics2D.IgnoreCollision(currentColliders[0], otherColliders[0], ignore);
                }
            }
        }
    }
}