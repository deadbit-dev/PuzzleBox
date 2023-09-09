using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class LoadBarSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent>> filter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LevelCmp>> levelFilter = Worlds.PresentationWorld;

        private EcsPoolInject<LoadLevelEvent> loadLevelEventPool = Worlds.PresentationWorld;
        private EcsPoolInject<LevelCmp> levelCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<BarTag> barTagPool = Worlds.PresentationWorld;
        private EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            if (filter.Value.GetEntitiesCount() <= 0) return;

            foreach (var levelEntity in levelFilter.Value)
            {
                int entity = world.Value.NewEntity();
                transformRefCmpPool.Value.Add(entity).Value = levelCmpPool.Value.Get(levelEntity).UI.Bar.transform;
                barTagPool.Value.Add(entity);
                levelElementCmpPool.Value.Add(entity).LevelIndex = loadLevelEventPool.Value.Get(levelEntity).LevelIndex;
            }
        }
    }
}