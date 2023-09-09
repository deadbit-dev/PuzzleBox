using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class LoadRestartLevelButtonSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent, LevelCmp>> filter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<LoadLevelEvent> loadLevelEventPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LevelCmp> levelCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<RestartLevelButtonTag> restartLevelButtonTagPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var levelEntity in filter.Value)
            {
                ref var restartButton = ref levelCmpPool.Value.Get(levelEntity).UI.RestartButton;

                var entity = world.Value.NewEntity();
                transformRefCmpPool.Value.Add(entity).Value = restartButton.transform;
                levelElementCmpPool.Value.Add(entity).LevelIndex = loadLevelEventPool.Value.Get(levelEntity).LevelIndex;
                restartLevelButtonTagPool.Value.Add(entity);
                restartButton.Init(world.Value, world.Value.PackEntity(entity));
            }
        }
    }
}