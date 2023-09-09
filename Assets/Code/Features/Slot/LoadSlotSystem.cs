using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class LoadSlotSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent, LevelCmp>> filter = Worlds.PresentationWorld;

        private EcsPoolInject<LoadLevelEvent> loadLevelEventPool = Worlds.PresentationWorld;
        private EcsPoolInject<LevelCmp> levelCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<SlotCmp> slotCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach(var levelEntity in filter.Value)
            {
                ref LevelCmp levelCmp = ref levelCmpPool.Value.Get(levelEntity);

                ref Transform[] slots = ref levelCmp.UI.Bar.Slots;
                for (var i=0; i < slots.Length; i++)
                {
                    int entity = world.Value.NewEntity();
                    ref SlotCmp slotCmp = ref slotCmpPool.Value.Add(entity);
                    slotCmp.Transform = slots[i].parent;
                    slotCmp.Origin = slots[i];

                    levelElementCmpPool.Value.Add(entity).LevelIndex = loadLevelEventPool.Value.Get(levelEntity).LevelIndex;
                }
            }
        }
    }
}