using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class BounceZoneSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TransformRefCmp, ProjectileCmp, CollisionEnterEvent>> projectileFilter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<TransformRefCmp, BounceZoneCmp>> bounceZoneFilter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<BounceZoneCmp> bounceZoneCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<CollisionEnterEvent> collisionEnterEventPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach(var projectileEntity in projectileFilter.Value)
            {
                ref var collisionEnterEvent = ref collisionEnterEventPool.Value.Get(projectileEntity);

                foreach(var bounceZoneEntity in bounceZoneFilter.Value)
                {
                    ref var transformRefCmp = ref transformRefCmpPool.Value.Get(bounceZoneEntity);
                    ref var bounceZoneCmp = ref bounceZoneCmpPool.Value.Get(bounceZoneEntity);

                    if (collisionEnterEvent.Value.transform != transformRefCmp.Value) continue;

                    collisionEnterEvent.Value.otherRigidbody.velocity *= bounceZoneCmp.Force;
                }
            }
        }
    }
}