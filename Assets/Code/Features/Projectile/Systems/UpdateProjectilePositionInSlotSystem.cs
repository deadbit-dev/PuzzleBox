using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class UpdateProjectilePositionInSlotSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<PositionTweenCmp, LinkedToSlot>, Exc<DraggingTag>> filter = Worlds.PresentationWorld;
        private EcsPoolInject<PositionTweenCmp> positionTweenCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<SlotCmp > slotCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<LinkedToSlot> linkedToSlotPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach(var projectileEntity in filter.Value)
            {
                ref PositionTweenCmp positionTweenCmp = ref positionTweenCmpPool.Value.Get(projectileEntity);
                ref LinkedToSlot linkedToSlot = ref linkedToSlotPool.Value.Get(projectileEntity);

                var slot_is_alive = linkedToSlot.Value.Unpack(world.Value, out int entity);
                if (!slot_is_alive) continue;

                ref SlotCmp slotCmp = ref slotCmpPool.Value.Get(entity);
                positionTweenCmp.EndPosition = slotCmp.Transform.position;
            }
        }
    }
}