using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class RotationHandlerActivationSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<TransformRefCmp, RotationHandlerCmp, LinkedToShotPointCmp>> rotationHandlerFilter = Worlds.PresentationWorld;

        private EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<LinkedToShotPointCmp> linkedToShotPointPool = Worlds.PresentationWorld;
        private EcsPoolInject<AttachedProjectile> attachedProjectilePool = Worlds.PresentationWorld;
        private EcsPoolInject<AimingTag> aimingTagPool = Worlds.PresentationWorld;
        private EcsPoolInject<ActiveTag> activeTagPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var rotationHandlerEntity in rotationHandlerFilter.Value)
            {
                ref TransformRefCmp transformRefCmp = ref transformRefCmpPool.Value.Get(rotationHandlerEntity);
                ref LinkedToShotPointCmp linkedToShotPointCmp = ref linkedToShotPointPool.Value.Get(rotationHandlerEntity);

                var shotPoint_is_alive = linkedToShotPointCmp.Value.Unpack(world.Value, out int shotPointEntity);
                if (!shotPoint_is_alive) continue;
                
                var is_active = attachedProjectilePool.Value.Has(shotPointEntity) && !aimingTagPool.Value.Has(shotPointEntity);
                transformRefCmp.Value.gameObject.SetActive(is_active);

                if (is_active && !activeTagPool.Value.Has(rotationHandlerEntity)) activeTagPool.Value.Add(rotationHandlerEntity);
                else if (!is_active && activeTagPool.Value.Has(rotationHandlerEntity)) activeTagPool.Value.Del(rotationHandlerEntity);
            }
        }
    }
}