using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class LoadBounceZoneSystem : LoadPhysicsLevelObjectBase<BounceZone>
    {
        private readonly EcsPoolInject<BounceZoneCmp> bounceZoneCmpPool = Worlds.PresentationWorld;

        protected override void Load(int entity, BounceZone presenter)
        {
            base.Load(entity, presenter);
            bounceZoneCmpPool.Value.Add(entity).Force = presenter.Force;
        }
    }
}