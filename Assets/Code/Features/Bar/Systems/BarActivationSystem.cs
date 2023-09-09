using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class BarActivationSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TransformRefCmp, BarTag>> barFilter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<SlotCmp, ActiveTag>> activeSlotFilter = Worlds.PresentationWorld;

        private EcsPoolInject<TransformRefCmp> viewCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in barFilter.Value)
            {
                var has_active = (activeSlotFilter.Value.GetEntitiesCount() != 0);
                viewCmpPool.Value.Get(entity).Value.gameObject.SetActive(has_active);
            }
        }
    }
}